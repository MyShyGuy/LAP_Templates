using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DB_Models.DTO;
using DB_Models.Models;

namespace BLDAL.Repos
{
    public interface ITRegisterRepo
    {
        public bool RegisterMember(TRegisterDTO member);  
        public Task<bool> RegisterMemberAsync(TRegisterDTO member);      
    }
}