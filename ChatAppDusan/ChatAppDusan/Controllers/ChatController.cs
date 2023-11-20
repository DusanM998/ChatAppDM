using ChatAppDusan.Dtos;
using ChatAppDusan.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppDusan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("register-user")]
        public IActionResult RegisterUser(UserDto user)
        {
            if(_chatService.AddUserToList(user.Name))
            {
                return NoContent();
            }
            return BadRequest("Korisnicno ime je zauzeto, molimo Vas odaberite novo!");
        }
    }
}
