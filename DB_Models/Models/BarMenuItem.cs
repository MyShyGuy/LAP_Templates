using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_Models.Models
{
    [Table("BarMenuItems", Schema = "disp")]
    public class BarMenuItem
    {
        [Key]
        public int MenuItemID { get; set; }

        [StringLength(120)]
        public required string Title { get; set; }

        [StringLength(320)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
