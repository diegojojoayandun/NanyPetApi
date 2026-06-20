using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace NanyPetAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task JoinAppointmentRoom(string appointmentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, appointmentId);
        }

        public async Task LeaveAppointmentRoom(string appointmentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, appointmentId);
        }

        public async Task SendMessage(string appointmentId, string content)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? Context.User?.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(content)) return;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId || u.Id == userId);
            if (user == null) return;

            var message = new Message
            {
                AppointmentId = appointmentId,
                SenderId = user.Id,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.Group(appointmentId).SendAsync("ReceiveMessage", new
            {
                id = message.Id,
                appointmentId = message.AppointmentId,
                senderId = message.SenderId,
                senderName = $"{user.FirstName} {user.LastName}".Trim(),
                content = message.Content,
                sentAt = message.SentAt,
                isRead = false
            });
        }

        public async Task MarkAsRead(string appointmentId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? Context.User?.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userId)) return;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId || u.Id == userId);
            if (user == null) return;

            var unread = await _context.Messages
                .Where(m => m.AppointmentId == appointmentId && m.SenderId != user.Id && !m.IsRead)
                .ToListAsync();

            unread.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();

            await Clients.Group(appointmentId).SendAsync("MessagesRead", appointmentId, user.Id);
        }
    }
}
