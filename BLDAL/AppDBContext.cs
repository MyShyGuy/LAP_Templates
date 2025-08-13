using DB_Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace BLDAL
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        { }
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Database = WatchDogDB;Integrated Security = True;Connect Timeout = 30;Encrypt = False;Trust Server Certificate = True");
        //     }
        // } // Jetzt mit DI

        // für die migration in eine DB muss im Package Manager Console der Befehl "add-migration InitialDatabaseCreation -Project BLDAL -StartupProject BLDAL" ausgeführt werden
        // und danach der Befehl "update-database -Project BLDAL -StartupProject BLDAL"
        // um die migration zurück zusetzten kann man "Remove-Migration" verwenden
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(c => c.UserName)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(c => c.RoleName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleUser",
                    j => j.HasOne<Role>()
                          .WithMany()
                          .HasForeignKey("RoleID")
                          .OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<User>()
                          .WithMany()
                          .HasForeignKey("UserID")
                          .OnDelete(DeleteBehavior.Cascade));
        }
    }
}
