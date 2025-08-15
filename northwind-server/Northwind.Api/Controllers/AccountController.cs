using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Northwind.Api.Configuration;
using Northwind.Application.DTOs;
using Northwind.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginRequest = Northwind.Application.DTOs.LoginRequest;

namespace Northwind.Api.Controllers
{

   
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        // Dependency injection for the account service
        private readonly IAccountService _accountService;
        private IOptions<AppSettings> _appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="accountService">The account service.</param>
        public AccountController(IAccountService accountService, IOptions<AppSettings> appSettings)
        {
            _accountService = accountService;
            _appSettings = appSettings;
        }


        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The user registration request.</param>
        /// <returns>Registered user details.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                var createdUser = await _accountService.Register(request, request.Password);

                return CreatedAtAction(nameof(Register), new { id = createdUser.Id }, createdUser);
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, new { Message = "Database error occurred.", Details = sqlEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during registration.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">Login request containing email and password.</param>
        /// <returns>JWT token and user details.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _accountService.Login(request.Email, request.Password);
                if (user == null)
                    return Unauthorized(new { Message = "Invalid email or password." });

                var token = GenerateJwtToken(user.Email);

                return Ok(new LoginResponse
                {
                    Token = token,
                    UserDetails = user
                });
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, new { Message = "Database error occurred.", Details = sqlEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Generates a JWT token for the specified email.
        /// </summary>
        /// <param name="email">The email for which the token is generated.</param>
        /// <returns>A JWT token as a string.</returns>
        private string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _appSettings.Value.Secret;
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
                Expires = DateTime.UtcNow.AddHours(_appSettings.Value.ExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
