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
        private TodoRepo _todoRepo;
        private TRegisterRepo _TRegisterRepo;
        private DisplayRepo _displayRepo;
        public UnitOfWork(AppDBContext context)
        {
            dbc = context;
        }

        public UserRepo Userrepo => _usrRepo ??= new UserRepo(dbc);
        public TodoRepo TodoRepo => _todoRepo ??= new TodoRepo(dbc);
        public TRegisterRepo TRegisterRepo => _TRegisterRepo ??= new TRegisterRepo(dbc);
        public DisplayRepo DisplayRepo => _displayRepo ??= new DisplayRepo(dbc);

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
