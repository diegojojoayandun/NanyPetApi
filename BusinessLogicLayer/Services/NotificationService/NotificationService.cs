using DataAccessLayer.Data;
using DataAccessLayer.Entities.Enums;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DbNotification = DataAccessLayer.Entities.Notification;

namespace BusinessLogicLayer.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext context, IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;

            // Inicializar Firebase solo una vez
            if (FirebaseApp.DefaultInstance == null)
            {
                var serviceAccountJson = configuration["FIREBASE_SERVICE_ACCOUNT_JSON"];
                if (!string.IsNullOrEmpty(serviceAccountJson))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(serviceAccountJson)
                    });
                }
            }
        }

        public async Task SendPushAsync(string userId, string title, string body, NotificationType type, string? relatedId = null)
        {
            await SaveNotificationAsync(userId, title, body, type, relatedId);

            var user = await _context.Users.FindAsync(userId);
            if (user?.FcmToken == null) return;

            try
            {
                if (FirebaseApp.DefaultInstance == null) return;

                var message = new FirebaseAdmin.Messaging.Message
                {
                    Token = user.FcmToken,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = new Dictionary<string, string>
                    {
                        { "type", type.ToString() },
                        { "relatedId", relatedId ?? "" }
                    }
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error enviando notificación push a userId {UserId}", userId);
            }
        }

        public async Task SaveNotificationAsync(string userId, string title, string body, NotificationType type, string? relatedId = null)
        {
            var notification = new DbNotification
            {
                UserId = userId,
                Title = title,
                Body = body,
                Type = type,
                RelatedEntityId = relatedId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
