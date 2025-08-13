using BLDAL.Repos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLDAL
{
    public class UnitOfWork : IDisposable
    {
        private AppDBContext dbc;
        private UserRepo _usrRepo;
        public UnitOfWork(AppDBContext context)
        {
            dbc = context;
        }

        public UserRepo Userrepo => _usrRepo ??= new UserRepo(dbc);

        public int Commit()
        {
            return dbc.SaveChanges();
        }

        public void Dispose()
        {
            dbc.Dispose();
        }

        public void RollBack()
        {
            // RollBack: Alle Änderungen verwerfen
            foreach (var entry in dbc.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached; // Änderungen verwerfen
            }
        }

    }
}
