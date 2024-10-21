using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using OpenAI.Chat;

namespace SemanticAIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemanticController : ControllerBase
    {
        private  Kernel _kernel;
        public SemanticController(Kernel kernel) {

            _kernel = kernel;
                }
        [HttpGet("GetAI")]
        public IActionResult GetAI()
        {
            Console.WriteLine("Enter your message to Batman");
        var result = _kernel.InvokePromptAsync("Hii Hello").Result;


            return Ok(result.ToString());
        }
    }
}
