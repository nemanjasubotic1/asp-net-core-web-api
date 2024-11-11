using JustAnother.Model.Entity.DTO;

namespace JustAnother.API.Authorize;

public interface IUserRepository
{
    bool IsUniqueUser(string username);
    Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
}
