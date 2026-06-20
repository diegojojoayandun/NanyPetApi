using DataAccessLayer.Entities.Enums;

namespace BusinessLogicLayer.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendPushAsync(string userId, string title, string body, NotificationType type, string? relatedId = null);
        Task SaveNotificationAsync(string userId, string title, string body, NotificationType type, string? relatedId = null);
    }
}
