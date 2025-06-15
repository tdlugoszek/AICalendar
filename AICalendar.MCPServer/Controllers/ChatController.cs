using AICalendar.MCPServer.Services;
using Microsoft.AspNetCore.Mvc;
using AICalendar.Shared.Models;

namespace AICalendar.MCPServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IAIService _aiService;

        public ChatController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("stream")]
        public async IAsyncEnumerable<string> StreamChat([FromBody] ChatRequest request)
        {
            await foreach (var response in _aiService.GetStreamingResponseAsync(request.Message))
            {
                yield return response;
            }
        }
    }
}
