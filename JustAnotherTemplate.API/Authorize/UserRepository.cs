using AutoMapper;
using JustAnother.DataAccess;
using JustAnother.DataAccess.Data;
using JustAnother.Model;
using JustAnother.Model.Entity;
using JustAnother.Model.Entity.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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

            var jtwTokenId = $"Jti{Guid.NewGuid()}";

            TokenDTO tokenDTO = new()
            {
                AccessToken = CreateAccessToken(user, jtwTokenId, roles)
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
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
