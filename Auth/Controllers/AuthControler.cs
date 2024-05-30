using Microsoft.AspNetCore.Mvc;
using Auth.DTOs;
using Auth.Services;
using Auth.Models;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDTO registrationDTO)
        {
            var tokenResponse = await _authService.Register(registrationDTO);
            return Ok(tokenResponse);
        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(UserLoginDTO loginDTO)
        {
            var tokenResponse = await _authService.SignIn(loginDTO);
            return Ok(tokenResponse);
        }
    }
}
