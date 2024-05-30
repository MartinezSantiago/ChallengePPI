using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Auth.Models;
using Auth.DTOs;
using BCrypt.Net;
using Auth.Context;

namespace Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _dbContext;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthService(AuthDbContext dbContext, string secretKey, string issuer, string audience)
        {
            _dbContext = dbContext;
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public async Task<JwtTokenResponse> Register(UserRegistrationDTO registrationDTO)
        {
            // Check if email already exists
            if (await _dbContext.Users.AnyAsync(u => u.Email == registrationDTO.Email))
            {
                throw new Exception("Email already exists");
            }

            // Hash the password using bcrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationDTO.Password);

            // Save user to database
            var user = new User { Email = registrationDTO.Email, PasswordHash = passwordHash };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Generate JWT token
            string token = GenerateJwtToken(user.Id,user.Email);
            return new JwtTokenResponse { Token = token };
        }

        public async Task<JwtTokenResponse> SignIn(UserLoginDTO loginDTO)
        {
            // Find user by email
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }

            // Generate JWT token
            string token = GenerateJwtToken(user.Id,user.Email);
            return new JwtTokenResponse { Token = token };
        }

        private string GenerateJwtToken(int id,string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,id.ToString()),
                    new Claim(ClaimTypes.Name, email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
