using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace JwtInDotnetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ServerDbContext _context;

        public HomeController(IConfiguration config, ServerDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user =await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null || !VerifyPassword(user.Password, loginRequest.Password))
            {
                return Unauthorized("Invalid username or password.");
            }
            
            var token = GenerateJwtToken(user);
            return Ok(new { token= token, userId=user.Id });
        }

        [HttpPost("Signup")]
        public async Task <IActionResult> Signup([FromBody] User signupRequest)
        {
            Console.WriteLine("*******");
            Console.WriteLine(signupRequest);
            if (await _context.Users.AnyAsync(u => u.Username == signupRequest.Username))
            {
                return Conflict("Username already exists.");
            }

            var user = new User
            {
                Username = signupRequest.Username,
                Password = HashPassword(signupRequest.Password),
                Email= signupRequest.Email,
                Role= signupRequest.Role
                // Add any additional fields you want to save during signup
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "Signup successful!" });
        }
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(120), // Token expires after 120 minutes
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return token;
        }

        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

    }
}
