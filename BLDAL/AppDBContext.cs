using DB_Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace BLDAL
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) // benötigt für DI normale migration dieses projektes will mit dem constructor nicht funktionieren
        { }
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<TodoItem> Todos { get; set; } = null!;
        public virtual DbSet<RankingEntry> RankingLists { get; set; } = null!;


        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Database = TemplateDB;Integrated Security = True;Connect Timeout = 30;Encrypt = False;Trust Server Certificate = True");
        //     }
        // } // Jetzt mit DI

        // für die migration in eine DB muss im Package Manager Console der Befehl "add-migration InitialDatabaseCreation -Project BLDAL -StartupProject BLDAL" ausgeführt werden
        // und danach der Befehl "update-database -Project BLDAL -StartupProject BLDAL"
        // um die migration zurück zusetzten kann man "Remove-Migration" verwenden

        //man kann die migration auch über vscode mit dem dotnet befehl machen
        //dotnet tool install --global dotnet-ef
        //dotnet tool update --global dotnet-ef
        //dotnet ef migrations add addnewclasses --project BLDAL --startup-project BlazorTemplate
        //dotnet ef database update --project BLDAL --startup-project BlazorTemplate
        //hier muss bedacht werden das dann die migration über DI läuft wärend mein connection string in den appsettings sind.


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

            modelBuilder.Entity<User>()
                .HasMany(u => u.Todos)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleID = 1,
                    RoleName = "Admin",
                    Notes = "Hat volle Zugriffsrechte"
                },
                new Role
                {
                    RoleID = 2,
                    RoleName = "Customer",
                    Notes = "Eingeschränkter Zugriff"
                },
                new Role
                {
                    RoleID = 3,
                    RoleName = "Guest",
                    Notes = "Nur Lese Zugriff"
                });


            //hier noch die beziehung zwischen todoitems und user einfügen aka User has many Todoitems
        }
    }
}
