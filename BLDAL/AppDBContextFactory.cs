using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace BLDAL
{
    public class AppDBContextFactory : IDesignTimeDbContextFactory<AppDBContext>
    {
        public AppDBContext CreateDbContext(string[] args)
        {
            var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
            Directory.CreateDirectory(dbDirectory);

            var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
            optionsBuilder.UseSqlite("Data Source=./data/koowebsite.db");

            return new AppDBContext(optionsBuilder.Options);
        }
    }
}
