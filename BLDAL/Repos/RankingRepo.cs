using DB_Models.DTO;
using DB_Models.Models;
using DB_Models.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLDAL.Repos
{
    public class RankingRepo : IRankingRepo
    {
        private readonly AppDBContext dbc;

        public RankingRepo(AppDBContext dbc)
        {
            this.dbc = dbc;
        }

        public void deleteRanking(RankingEntry Ranking)
        {
            throw new NotImplementedException();
        }

        public List<RankingEntry> GetRankings(User usr)
        {
            return dbc.RankingLists.Where(r => r.user.UserID == usr.UserID).ToList();
        }

        public void SaveRanking(RankingEntry Ranking)
        {
            dbc.RankingLists.Add(Ranking);
            dbc.SaveChanges();
        }
    }
}