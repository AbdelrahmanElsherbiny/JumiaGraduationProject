using Microsoft.AspNetCore.Mvc;

namespace Chat_GPT.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly ChatGptService _chatGptService;

        public ChatController(ChatGptService chatGptService)
        {
            _chatGptService = chatGptService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Message cannot be empty" });
            }

            var response = await _chatGptService.SendMessageAsync(request.Message);
            return Ok(new { reply = response });
        }
    }
}
    public class ChatRequest
    {
        public string Message { get; set; }
    
    }

