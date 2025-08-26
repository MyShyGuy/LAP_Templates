using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB_Models.Models;

namespace DB_Models.DTO
{
    public class TodoDTO
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a Title")]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedAt { get; set; }
        public User? User { get; set; }
    }
}
