using DataAccessLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public partial class Owner : AuditableBaseModel
    {
        public Owner()
        {
            Pets = new HashSet<Pet>();
        }
        [Required]
        [EmailAddress]
        public string EmailUser { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Location { get; set; }

        public virtual User UserNameNavigation { get; set; } = null!;
        public virtual ICollection<Pet> Pets { get; set; }
    }
}
