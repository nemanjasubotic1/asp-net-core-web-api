using Azure;
using JustAnother.API.Authorize;
using JustAnother.Model;
using JustAnother.Model.Entity;
using JustAnother.Model.Entity.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JustAnother.API.Controllers;

[Route("api/v{version:apiVersion}/User")]
[ApiVersionNeutral]
[ApiController]
public class UserController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    private ApiResponse _apiResponse;

    private readonly IUserRepository _userRepository;

    public UserController(RoleManager<IdentityRole> roleManager, IUserRepository userRepository)
    {
        _roleManager = roleManager;
        _apiResponse = new ApiResponse();
        _userRepository = userRepository;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
    {
        var tokenDto = await _userRepository.Login(loginRequestDTO);

        if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AccessToken))
        {

            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = ["Username or password are incorect"];

            return BadRequest(_apiResponse);
        }

        _apiResponse.IsSuccess = true;
        _apiResponse.Result = tokenDto;
        _apiResponse.StatusCode = HttpStatusCode.OK;

        return Ok(_apiResponse);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
    {
        bool ifUserNameUnique = _userRepository.IsUniqueUser(registrationRequestDTO.UserName);

        if (!ifUserNameUnique)
        {

            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = ["Username already exists"];

            return BadRequest(_apiResponse);
        }

        var user = await _userRepository.Register(registrationRequestDTO);

        if (user == null)
        {

            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = ["User not found"];

            return NotFound(_apiResponse);
        }

        _apiResponse.StatusCode = HttpStatusCode.OK;
        _apiResponse.IsSuccess = true;

        return Ok(_apiResponse);
    }


    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDTO)
    {
        if (ModelState.IsValid)
        {
            var tokenDTOResponse = await _userRepository.RefreshAccessToken(tokenDTO);

            if (tokenDTOResponse is null || string.IsNullOrEmpty(tokenDTOResponse.AccessToken))
            {

                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = ["Invalid token"];

                return BadRequest(_apiResponse);
            }
            else
            {

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = tokenDTOResponse;

                return Ok(_apiResponse);
            }

        }
        else
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.Result = "Invalid input";

            return BadRequest(_apiResponse);
        }
    }

    // to be called on LogOff
    [HttpPost("revokeRefreshToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO tokenDTO)
    {
        if (ModelState.IsValid)
        {
            await _userRepository.RevokeRefreshToken(tokenDTO);

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        _apiResponse.IsSuccess = false;
        _apiResponse.ErrorMessages = ["Invalid input"];
        _apiResponse.StatusCode = HttpStatusCode.BadRequest;

        return BadRequest(_apiResponse);
    }
}
