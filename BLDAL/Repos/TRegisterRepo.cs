using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DB_Models.DTO;
using DB_Models.Models;

namespace BLDAL.Repos
{
    public class TRegisterRepo : ITRegisterRepo
    {
        private readonly AppDBContext dbc;
        public TRegisterRepo(AppDBContext dbContext)
        {
            this.dbc = dbContext;
        }

        public bool RegisterMember(TRegisterDTO member)
        {
            bool MemberExist = dbc.MatchMembers.Where(x => x.PhoneNumber == member.PhoneNumber).Any();
            if (!MemberExist)
            {
                var newMember = new MatchMember
                {
                    Name = member.Name,
                    PhoneNumber = member.PhoneNumber,
                    Email = member.Email
                };
                dbc.MatchMembers.Add(newMember);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterMemberAsync(TRegisterDTO member)
        {
            bool MemberExist = dbc.MatchMembers.Where(x => x.PhoneNumber == member.PhoneNumber).Any();
            if (!MemberExist)
            {
                var newMember = new MatchMember
                {
                    Name = member.Name,
                    PhoneNumber = member.PhoneNumber,
                    Email = member.Email
                };
                dbc.MatchMembers.Add(newMember);
                dbc.SaveChanges();
                return true;
            }
            return false;
        }
        
    }
}