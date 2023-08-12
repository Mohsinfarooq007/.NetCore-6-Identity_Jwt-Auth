using julianIdentityApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace julianIdentityApi.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;

        public UserController(UserManager<IdentityUser> usermanager, IConfiguration config)
        {
            _userManager = usermanager;
            _configuration = config;
        }


        [AllowAnonymous]
        [HttpPost("registerUser")]
        public async Task RegisterUser(RegisterModel registerModel)
        {
            var identityUser = new IdentityUser
            {
                Email = registerModel.Email,
                UserName = registerModel.Name,

            };
            try
            {
                var response = await _userManager.CreateAsync(identityUser, registerModel.Password);

                if (response.Succeeded == true)
                {
                    await _userManager.AddToRoleAsync(identityUser, registerModel.Role);

                }
            }
            catch (Exception ex) { }

        }

        [AllowAnonymous]
        [HttpPost("loginUser")]
        public async Task<string> loginUser(string Email, string password)
        {

            try
            {
                IdentityUser identityUser = await _userManager.FindByEmailAsync(Email);
                if (identityUser != null)
                {
                    var result = await _userManager.CheckPasswordAsync(identityUser, password);

                    if (result == true)
                    {
                        IList<string> role = await _userManager.GetRolesAsync(identityUser);
                        // var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"));
                        // var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                        var claims = new List<Claim>
{
                               new Claim(ClaimTypes.Name, "John Doe"),
                                   new Claim(ClaimTypes.Email, "john.doe@example.com"),
                                   new Claim(ClaimTypes.Role,role[0]),
    // Add more claims as needed
};

                        string token=GenerateJwtToken("","","",0,claims);

                        return token;
                    }
                }
                return "something went wrong";

            }
            catch (Exception ex) { return "something went wrong"; }

        }

        public static string GenerateJwtToken(string secretKey, string issuer, string audience, int expiryMinutes, IEnumerable<Claim> claims)
        {
            secretKey = "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr";
            issuer = "https://localhost:7248";
            audience = "https://localhost:7248";
            expiryMinutes = 30;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [Authorize(Roles ="Manager")]
        [HttpGet("protected")]
        public IActionResult ProtectedAction()
        {
            return Ok("You're Authorized");
        }



    }
}
