using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.Common
{
    public abstract class AuditableBaseModel
    {
        [Key] // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // autoincrement
        public virtual string Id { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
