using DataAccessLayer.Entities.Common;

namespace DataAccessLayer.Entities
{
    public partial class Pet : AuditableBaseModel
    {
        public Pet()
        {
            Appointments = new HashSet<Appointment>();
        }

        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
        public string? OwnerId { get; set; }

        public virtual Owner Owner { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
