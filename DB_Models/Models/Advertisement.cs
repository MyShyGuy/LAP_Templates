using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_Models.Models
{
    [Table("Advertisements")]
    public class Advertisement
    {
        [Key]
        public int AdID { get; set; }

        [StringLength(120)]
        public required string Title { get; set; }

        [StringLength(260)]
        public required string ImagePath { get; set; }

        [Range(3, 300)]
        public int DurationSeconds { get; set; } = 10;

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
