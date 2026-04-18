using DB_Models.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_Models.Models
{
    [Table("MatchMembers")]
    public class MatchMember
    {
        [Key]
        public int MemberID { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}