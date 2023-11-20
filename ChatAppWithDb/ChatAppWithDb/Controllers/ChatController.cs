using ChatAppWithDb.DbContexts;
using ChatAppWithDb.Models;
using ChatAppWithDb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppWithDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        private readonly ApplicationDbContexts _db;
        public ChatController(ChatService chatService, ApplicationDbContexts db)
        {
            _chatService = chatService;
            _db = db;
        }

        [HttpPost("register-user")]
        public IActionResult RegisterUser(User user)
        {
            if (_chatService.AddUserToList(user.Name))
            {
                _db.Users.Add(user);
                _db.SaveChanges();
                return NoContent();
            }
            return BadRequest("Korisnicno ime je zauzeto, molimo Vas odaberite novo!");
        }

        [HttpPost("send-message")]
        public IActionResult SendMessage(Message message)
        {
            _db.Messages.Add(message);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpGet("get-messages")]
        public IActionResult GetMessages()
        {
            var messages = _db.Messages.ToList();
            return Ok(messages);
        }
    }
}
