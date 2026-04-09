using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BLDAL
{
    public class AppDBContextFactory : IDesignTimeDbContextFactory<AppDBContext>
    {
        public AppDBContext CreateDbContext(string[] args)
        {
            var connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=(localdb)\\MSSQLLocalDB;Database=BlazorTemplateDesignTime;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDBContext(optionsBuilder.Options);
        }
    }
}
