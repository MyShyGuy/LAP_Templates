using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DB_Models.Models
{
    [Table("Todos", Schema = "usr")]
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }
        [StringLength(150)]
        public required string Title { get; set; }
        [StringLength(400)]
        public string? Description { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }     // Fälligkeitsdatum optional

        // FK
        public int UserID { get; set; }
        public required User User { get; set; }    // Navigation Property
    }
}