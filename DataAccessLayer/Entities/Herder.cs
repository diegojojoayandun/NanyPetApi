using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public partial class Herder 
    {
        public Herder()
        {
            Appointments = new HashSet<Appointment>();
        }
        [Key] // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // autoincrement
        public  string Id { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string EmailUser { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Location { get; set; }

        public virtual User UserNameNavigation { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
