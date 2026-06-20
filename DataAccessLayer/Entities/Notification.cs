using DataAccessLayer.Entities.Common;
using DataAccessLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public partial class Notification : AuditableBaseModel
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;

        public NotificationType Type { get; set; }

        public string? RelatedEntityId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}
