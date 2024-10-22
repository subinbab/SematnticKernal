using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SemanticAIApp.Models;
using SemanticAIApp.Repository;
using System.Security.Claims;

namespace SemanticAIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public OpenAIController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost("CreateOpenAIAccount")]
        [Authorize]
        public IActionResult CreateOpenAIAccount([FromBody]OpenAISubscription openAISubscription)
        {
            try
            {
                // Get the UserId from the JWT Token
                var userId = User.FindFirstValue("User_Id");
                // Fetch the user from the database using the userId
                var user =  _userManager.FindByIdAsync(userId).Result;
                openAISubscription.User = user;
                openAISubscription.UserId = user.Id;
                var createResult = _context.OpenAISubscription.Add(openAISubscription);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
    }
}
