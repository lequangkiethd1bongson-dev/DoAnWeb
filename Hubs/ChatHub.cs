using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAnWeb.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public static readonly Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>(); // UserId -> ConnectionId

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string messageContent)
        {
            var senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId)) return;

            if (string.IsNullOrEmpty(receiverId))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var firstAdmin = admins.FirstOrDefault();
                if (firstAdmin != null)
                {
                    receiverId = firstAdmin.Id;
                }
                else
                {
                    return; // No admin to receive
                }
            }

            // Save to database
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = messageContent,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };
            
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Broadcast to the specific receiver if connected
            if (!string.IsNullOrEmpty(receiverId) && ConnectedUsers.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, messageContent, message.Timestamp);
            }
            // If the sender is also active, emit back for UI consistency (optional, UI usually handles local append)
            // await Clients.Caller.SendAsync("MessageSent", message.Id, message.Timestamp);
        }
    }
}
