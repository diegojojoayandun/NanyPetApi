using DataAccessLayer.Entities.Common;

namespace DataAccessLayer.Entities
{
    public partial class Appointment : AuditableBaseModel
    {
        public int? AnimalId { get; set; }
        public string? PetId { get; set; }
        public string? HerderId { get; set; }
        public DateTime? AppointmentTime { get; set; }
        public string? Notes { get; set; }

        public virtual Herder? Herder { get; set; }
        public virtual Pet? Pet { get; set; }
    }
}
