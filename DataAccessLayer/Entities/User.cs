using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Entities
{
    public partial class User : IdentityUser
    {
        public User()
        {
            Herders = new HashSet<Herder>();
            Notifications = new HashSet<Notification>();
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Token para notificaciones push
        public string? FcmToken { get; set; }

        public virtual Owner? Owner { get; set; }
        public virtual ICollection<Herder> Herders { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
