using DB_Models.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_Models.Models
{
    [Table("Ranking", Schema = "rkg")]
    public class RankingEntry
    {
        [Key]
        public int RankID { get; set; }
        public User user { get; set; }
        public TimeSpan time { get; set; }
        public DateTime PlayedAtDate { get; set; }
    }
}