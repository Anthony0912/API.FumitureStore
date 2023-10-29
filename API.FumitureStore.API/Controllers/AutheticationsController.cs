using API.FumitureStore.API.Configuration;
using API.FumitureStore.Shared.Auth;
using API.FumitureStore.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.FumitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutheticationsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AutheticationsController(UserManager<IdentityUser> userManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Verificar si el email existe
            var emailExist = await _userManager.FindByEmailAsync(request.EmailAddress);
            if (emailExist != null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email already exists"
                    }
                });
            }

            //crear el usuario
            var user = new IdentityUser()
            {
                Email = request.EmailAddress,
                UserName = request.EmailAddress
            };

            var isCreated = await _userManager.CreateAsync(user, request.Password);
            if (isCreated.Succeeded)
            {
                var token = GenerateToken(user);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });
            }
            var errors = new List<string>();
            foreach (var err in isCreated.Errors)
            {
                errors.Add(err.Description);
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = errors,
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser == null)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string>() { "Invalid Email or Password" }
                });
            }

            var checkUserAndPass = await _userManager.CheckPasswordAsync(existingUser, request.Password);

            if (!checkUserAndPass)
            {
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string>() { "Invalid Email or Password" }
                });
            }

            var token = GenerateToken(existingUser);

            return Ok(new AuthResult { Token = token, Result = true });
        }

        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]{
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
    }

}
