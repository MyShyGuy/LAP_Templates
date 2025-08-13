using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DB_Models.Models
{
    [Table("Users", Schema = "usr")]
    public class User
    {
        public User() { }

        [Key]
        public required string UserID { get; set; }

        [StringLength(150)]
        public required string UserName { get; set; }

        [StringLength(64)]
        public required string PasswordHash { get; set; }

        public required DateTime EntryDate { get; set; }


        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    }
}
