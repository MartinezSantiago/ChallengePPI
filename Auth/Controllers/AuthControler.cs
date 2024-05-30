using Microsoft.AspNetCore.Mvc;
using Auth.DTOs;
using Auth.Services;
using Auth.Models;

namespace Auth.Controllers
{
    /// <summary>
    /// Controller for authentication operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registrationDTO">The user registration data.</param>
        /// <returns>The token response.</returns>
        /// <response code="200">Returns the token response if registration is successful.</response>
        /// <response code="400">If the registration data is invalid or email already exists.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDTO registrationDTO)
        {
            try
            {
                var tokenResponse = await _authService.Register(registrationDTO);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Signs in a user.
        /// </summary>
        /// <param name="loginDTO">The user login data.</param>
        /// <returns>The token response.</returns>
        /// <response code="200">Returns the token response if sign-in is successful.</response>
        /// <response code="400">If the login data is invalid or email not found or password incorrect.</response>
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(UserLoginDTO loginDTO)
        {
            try
            {
                var tokenResponse = await _authService.SignIn(loginDTO);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
