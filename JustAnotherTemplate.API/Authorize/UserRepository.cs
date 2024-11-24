using AutoMapper;
using JustAnother.API.Utility;
using JustAnother.DataAccess;
using JustAnother.DataAccess.Data;
using JustAnother.Model.Entity;
using JustAnother.Model.Entity.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JustAnother.API.Authorize
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private string secretKey = "";

        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext db, IMapper mapper, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
            secretKey = configuration.GetValue<string>("ApiSetting:SecretKey");
            _userManager = userManager;
        }

        #region RegisterLogin

        public bool IsUniqueUser(string username)
        {
            var applicationUser = _db.ApplicationUsers.FirstOrDefault(l => l.UserName == username);

            return applicationUser == null;

        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(l => l.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            if (user == null)
            {
                return new TokenDTO()
                {
                    AccessToken = "",
                    RefreshToken = ""
                };
            }

            bool isValidUser = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!isValidUser)
            {
                return new TokenDTO()
                {
                    AccessToken = "",
                    RefreshToken = ""
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            var jwtTokenId = $"Jti{Guid.NewGuid()}";

            var refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return new TokenDTO()
                {
                    AccessToken = "",
                    RefreshToken = ""
                };
            }

            TokenDTO tokenDTO = new()
            {
                AccessToken = CreateAccessToken(user, jwtTokenId, roles),
                RefreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId)
            };

            return tokenDTO;
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.UserName,
                Email = registrationRequestDTO.UserName,
                NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
                Name = registrationRequestDTO.Name,
            };

            try
            {
                var registerResult = await _userManager.CreateAsync(user, registrationRequestDTO.Password);

                if (registerResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registrationRequestDTO.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registrationRequestDTO.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.Role_User);
                    }

                    var userToReturn = await _db.ApplicationUsers.FirstOrDefaultAsync(l => l.UserName == registrationRequestDTO.UserName);

                    if (userToReturn != null)
                    {
                        return new UserDTO
                        {
                            Name = userToReturn.UserName,
                            Username = userToReturn.UserName,
                            Id = userToReturn.Id
                        };
                    }

                    return new UserDTO();
                }

                return new UserDTO();
            }
            catch (Exception)
            {
                return new UserDTO();
            }
        }

        #endregion

        #region AccessAndRefreshToken

        private string CreateAccessToken(ApplicationUser user, string jwtTokenId, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var issuer = _configuration.GetValue<string>("ApiSetting:Issuer");
            var audience = _configuration.GetValue<string>("ApiSetting:Audience");

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Iss, issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, audience),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private async Task<string> CreateNewRefreshToken(string userId, string accessTokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = accessTokenId,
                ExipresAt = DateTime.UtcNow.AddMinutes(10), // must be in future date
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
            };

            if (!ModelValidator.TryValidate(refreshToken, out List<ValidationResult>? validationResults))
            {
                return "";
            }

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();

            return refreshToken.Refresh_Token;
        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            //Find existing Refresh Token in Database
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(l => l.Refresh_Token == tokenDTO.RefreshToken);

            if (existingRefreshToken == null)
            {
                return new TokenDTO();
            }

            //Compare AccessToken that is submited to AccessToken from Database, Table RefreshTokens 
            var isValidSubmitedAccessToken = IsAccessTokenValid(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

            if (!isValidSubmitedAccessToken)
            {
                // invalidate RefreshToken
                await MarkTokenAsInvalid(existingRefreshToken);

                return new TokenDTO();
            }

            //Check whether Refresh Token IsValid
            if (!existingRefreshToken.IsValid)
            {
                await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.Refresh_Token);

                return new TokenDTO();
            }

            // Check if Refresh Token is expired
            if (existingRefreshToken.ExipresAt < DateTime.UtcNow)
            {
                await MarkTokenAsInvalid(existingRefreshToken);

                return new TokenDTO();
            }

            // Replace existing Refresh Token with newly created Refresh Token, reset expire time
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

            if (string.IsNullOrEmpty(newRefreshToken))
            {
                return new TokenDTO();
            }

            //Revoke existing Refresh Token (invalidate)
            await MarkTokenAsInvalid(existingRefreshToken);

            //Generate new Access Token Finally!
            var applicationUser = _db.ApplicationUsers.FirstOrDefault(l => l.Id == existingRefreshToken.UserId);
            var roles = await _userManager.GetRolesAsync(applicationUser);

            if (applicationUser == null)
            {
                return new TokenDTO();
            }

            var newAccessToken = CreateAccessToken(applicationUser, existingRefreshToken.JwtTokenId, roles);

            return new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }


        public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        {
            // Find an existing Refresh Token
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(l => l.Refresh_Token == tokenDTO.RefreshToken);

            if (existingRefreshToken == null)
                return;

            var accessTokenData = IsAccessTokenValid(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!accessTokenData)
            {
                return;
            }

            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.Refresh_Token);
        }

        #endregion

        #region RefreshTokenHelperMethods

        private bool IsAccessTokenValid(string submitedAccessToken, string existingDbUserId, string existingDbAccessTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var jwt = tokenHandler.ReadJwtToken(submitedAccessToken);

                var submitedAccessTokenIdFromTokenClaim = jwt.Claims.FirstOrDefault(l => l.Type == JwtRegisteredClaimNames.Jti).Value;
                var submitedUserIdFromTokenClaims = jwt.Claims.FirstOrDefault(l => l.Type == JwtRegisteredClaimNames.Sub).Value;

                return submitedUserIdFromTokenClaims == existingDbUserId && existingDbAccessTokenId == submitedAccessTokenIdFromTokenClaim;

            }
            catch (Exception)
            {
                return false;

            }
        }

        private async Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            await _db.SaveChangesAsync();
        }

        private async Task MarkAllTokenInChainAsInvalid(string userId, string refreshToken)
        {
            await _db.RefreshTokens.Where(l => l.UserId == userId && l.Refresh_Token == refreshToken)
                  .ExecuteUpdateAsync(l => l.SetProperty(r => r.IsValid, false));
        }

        #endregion
    }
}
