using ChatAppBE.Data;
using ChatAppBE.Dto;
using ChatAppBE.Hubs;
using ChatAppBE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBE.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatController(AppDbContext context, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _chatHub = chatHub;
        }

        [HttpPost("messages")]
        public async Task<IActionResult> GetMessages([FromBody] MessageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Sender) || string.IsNullOrWhiteSpace(request.Receiver))
            {
                Console.WriteLine("❌ DEBUG: Missing sender or receiver in request");
                return BadRequest(new { message = "Both sender and receiver are required." });
            }

            Console.WriteLine($"🛠 DEBUG: Fetching messages between {request.Sender} and {request.Receiver}");

            var messages = await _context.Messages
                .Where(m => (m.Sender == request.Sender && m.Receiver == request.Receiver) ||
                            (m.Sender == request.Receiver && m.Receiver == request.Sender))
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    Sender = m.Sender,
                    Receiver = m.Receiver,
                    Content = string.IsNullOrWhiteSpace(m.Content) ? "[No Content]" : m.Content, // ✅ Nếu `Content` bị null, thay thế
                    Timestamp = m.Timestamp
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Sender) || string.IsNullOrEmpty(request.Receiver) || string.IsNullOrEmpty(request.Content))
            {
                return BadRequest(new { message = "Sender, Receiver, and Message are required." });
            }

            var newMessage = new Message
            {
                Sender = request.Sender,
                Receiver = request.Receiver,
                Content = request.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Gửi tin nhắn qua SignalR
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", request.Sender, request.Receiver, request.Content);

            return Ok(new { message = "Message sent successfully" });
        }

    }
}