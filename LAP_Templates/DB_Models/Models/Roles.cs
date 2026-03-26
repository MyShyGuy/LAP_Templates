using DB_Models.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_Models.Models
{
    [Table("Roles", Schema = "usr")]
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [StringLength(50)]
        public required string RoleName { get; set; }
        [StringLength(200)]
        public string? Notes { get; set; }


        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
