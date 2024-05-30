using Auth.DTOs;
using Auth.Models;

namespace Auth.Services
{
    public interface IAuthService
    {
        Task<JwtTokenResponse> Register(UserRegistrationDTO registrationDTO);
        Task<JwtTokenResponse> SignIn(UserLoginDTO loginDTO);
    }
}
