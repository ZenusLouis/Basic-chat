using ChatAppBE.Data;
using ChatAppBE.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ChatAppBE.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private static ConcurrentDictionary<string, string> UserConnections = new();

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name; // Lấy username từ JWT
            if (!string.IsNullOrEmpty(username))
            {
                UserConnections[username] = Context.ConnectionId; // Lưu ConnectionId
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(username))
            {
                UserConnections.TryRemove(username, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string sender, string receiver, string message)
        {
            if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(receiver) || string.IsNullOrEmpty(message))
            {
                throw new HubException("Sender, Receiver and Message cannot be empty.");
            }

            var newMessage = new Message
            {
                Sender = sender,
                Receiver = receiver,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Gửi tin nhắn đến người nhận
            if (UserConnections.TryGetValue(receiver, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", sender, receiver, message);
            }

            // Gửi tin nhắn đến chính người gửi để cập nhật UI ngay
            if (UserConnections.TryGetValue(sender, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", sender, receiver, message);
            }
        }
    }
}
