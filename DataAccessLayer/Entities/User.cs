using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Entities
{
    public partial class User : IdentityUser
    {
        public User()
        {
            Herders = new HashSet<Herder>();
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public virtual Owner? Owner { get; set; }
        public virtual ICollection<Herder> Herders { get; set; }
    }
}
