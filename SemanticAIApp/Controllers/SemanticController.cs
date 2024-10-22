using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using OpenAI.Chat;
using SemanticAIApp.Models;
using SemanticAIApp.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SemanticAIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemanticController : ControllerBase
    {
        private Kernel _kernel;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public SemanticController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, Kernel kernel, AppDbContext context, IConfiguration configurationBuilder, RoleManager<IdentityRole> roleManager) {

            _kernel = kernel;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _config = configurationBuilder;
            _roleManager = roleManager;
        }
        [HttpGet("GetAI")]
        public IActionResult GetAI()
        {
            Console.WriteLine("Enter your message to Batman");
            var result = _kernel.InvokePromptAsync("Hii Hello").Result;


            return Ok(result.ToString());
        }
        [HttpPost("SignUp")]
        public IActionResult SignUp(SignUpDto signup)
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                // Create a new IdentityUser based on the incoming model data
                var user = new IdentityUser { UserName = signup.UserName, Email = signup.Email };
                // Attempt to create the user in the identity system
                var result = _userManager.CreateAsync(user, signup.Password).Result;
                // If creation succeeded, return a success message
                if (result.Succeeded)
                {
                    var addroleresult = _userManager.AddToRoleAsync(user, signup.Role).Result;
                    return Ok(new { Result = "User registered successfully" });
                }
                else
                {
                    return BadRequest(new { Result = String.Join(',', result.Errors.ToList().Select(c => c.Description)) });
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost("SignIn")]
        public IActionResult SignIn(string username , string password)
        {
            try
            {
                // Find the user based on the username
                var user =  _userManager.FindByNameAsync(username).Result;
                // Validate the user's credentials
                if (user != null &&  _userManager.CheckPasswordAsync(user,password).Result)
                {
                    // Generate the JWT token and return it
                    var token = GenerateJwtToken(user);
                    return Ok(new { Token = token });
                }
                else
                {
                    return Unauthorized("Invalid username or password");
                }
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
        // POST: api/Authentication/generate-sso-token
        [HttpPost("generate-sso-token")]
        [Authorize] // Ensures that the user is authenticated
        public async Task<IActionResult> GenerateSSOToken()
        {
            try
            {
                // Get the UserId from the JWT Token
                var userId = User.FindFirstValue("User_Id");
                // Fetch the user from the database using the userId
                var user = await _userManager.FindByIdAsync(userId);
                // Check if the user exists
                if (user == null)
                {
                    return NotFound("User not found");
                }
                // Create a new SSO token and add it to the database
                var ssoToken = new SSOToken
                {
                    UserId = user.Id,
                    Token = Request.Headers["Authorization"].ToString(), // Generate a unique token
                    ExpiryDate = DateTime.UtcNow.AddMinutes(10), // Set an expiration time for the token
                    IsUsed = false
                };
                // Add the token to the database
                _context.SSOTokens.Add(ssoToken);
                await _context.SaveChangesAsync();
                // Return the newly created SSO token
                return Ok(new { SSOToken = ssoToken.Token });
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur and return a server error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize]
        // POST: api/Authentication/validate-sso-token
        [HttpPost("validate-sso-token")]
        public async Task<IActionResult> ValidateSSOToken( string token)
        {
            try
            {
                // Validate the incoming model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // Fetch the SSO token from the database
                var ssoToken = _context.SSOTokens.SingleOrDefaultAsync(t => t.Token == token).Result;
                // Check if the token is valid (exists, not used, not expired)
                if (ssoToken == null || ssoToken.IsUsed || ssoToken.ExpiryDate < DateTime.UtcNow)
                {
                    return BadRequest("Invalid or expired SSO token");
                }
                // Mark the token as used
                ssoToken.IsUsed = true;
                _context.SSOTokens.Update(ssoToken);
                await _context.SaveChangesAsync();
                // Find the user associated with the SSO token
                var user = await _userManager.FindByIdAsync(ssoToken.UserId);
                // Generate a new JWT
                var newJwtToken = GenerateJwtToken(user);
                // Return the new JWT token along with user details (e.g., username and email)
                return Ok(new
                {
                    Token = newJwtToken,
                    UserDetails = new
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        UserId = user.Id
                    }
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a server error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // Helper method to generate a JWT token for the authenticated user
        private string GenerateJwtToken(IdentityUser user)
        {
            // Defines a set of claims to be included in the token.
            var claims = new List<Claim>
            {
                // Custom claim using the user's ID.
                new Claim("User_Id", user.Id.ToString()),
                
                // Standard claim for user identifier, using username.
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                
                // Standard claim for user's email.
                new Claim(ClaimTypes.Email, user.Email),
                
                // Standard JWT claim for subject, using user ID.
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };
            // Get the symmetric key from the configuration and create signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), // Token expiration time
                signingCredentials: creds);
            // Serialize the token and return it as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

