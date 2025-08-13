using DB_Models.DTO;
using DB_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLDAL.Repos
{
    public interface IUserRepo
    {
        User? RegisterUser(LoginDTO usr);
        User? UserLogin(LoginDTO usr);
    }
}
