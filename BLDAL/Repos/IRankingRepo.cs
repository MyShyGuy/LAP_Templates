using DB_Models.DTO;
using DB_Models.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLDAL.Repos
{
    public interface IRankingRepo
    {
        public List<RankingEntry> GetRankings(User usr);
        public void SaveRanking(RankingEntry Ranking);
        public void deleteRanking(RankingEntry Ranking);
    }
}
