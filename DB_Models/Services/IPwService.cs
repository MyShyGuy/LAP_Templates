using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Models.Services
{
    public interface IPwService
    {
        string ComputeHash(string password, string salt);
    }

}
