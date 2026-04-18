using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_Models.Models
{
    [Table("BarActions")]
    public class BarAction
    {
        [Key]
        public int ActionID { get; set; }

        [StringLength(120)]
        public required string Title { get; set; }

        [StringLength(500)]
        public string? Details { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
