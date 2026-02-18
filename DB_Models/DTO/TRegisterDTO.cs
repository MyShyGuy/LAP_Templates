using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Models.DTO
{
    public class TRegisterDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a User Name")]
        public string? Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a Phone Number")]
        public string? PhoneNumber { get; set; }
        
        public string? Email { get; set; }
    }
}
