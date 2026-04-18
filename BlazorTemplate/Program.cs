using BlazorTemplate.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using BLDAL;
using DB_Models.Models;
using DB_Models.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

var dbDirectory = Path.Combine(builder.Environment.ContentRootPath, "data");
Directory.CreateDirectory(dbDirectory);

var configuredConnectionString = builder.Configuration.GetConnectionString("MyDatabase");
var defaultDbPath = Path.Combine(dbDirectory, "koowebsite.db");
var connectionString = string.IsNullOrWhiteSpace(configuredConnectionString) ||
                       configuredConnectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase)
    ? $"Data Source={defaultDbPath}"
    : configuredConnectionString;

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // alte Provider entfernen
    logging.AddConsole();     // Konsole
    logging.AddDebug();       // Visual Studio Debug
    logging.SetMinimumLevel(LogLevel.Information); // Level
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.Name = "auth_token";
    options.LoginPath = "/login";
    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
    options.AccessDeniedPath = "/access-denied";
}); //this is cookie auth ---- not JWT

//// DbContext registrieren und Connection String aus appsettings.json laden
//builder.Services.AddDbContext<AppDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase"))
//);

// DbContextFactory registrieren -- added
builder.Services.AddDbContextFactory<AppDBContext>(options =>
{
    options.UseSqlite(connectionString, sqliteOptions =>
    {
        sqliteOptions.CommandTimeout(30);
    });
});

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Für die abfrage der Client IP über HTTPContext
builder.Services.AddHttpContextAccessor();

//Auth state weitergabe und einfügen der Unit of work
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<BLDAL.UnitOfWork>();

var app = builder.Build();

await SeedAdminUserAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task SeedAdminUserAsync(IServiceProvider services)
{
    const string adminUserName = "admin";
    const string adminPassword = "Rofl0815";
    const string adminRoleName = "Admin";

    using var scope = services.CreateScope();
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDBContext>>();
    await using var dbContext = await dbFactory.CreateDbContextAsync();

    await dbContext.Database.MigrateAsync();

    var adminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == adminRoleName);
    if (adminRole == null)
    {
        adminRole = new Role
        {
            RoleName = adminRoleName,
            Notes = "Hat volle Zugriffsrechte"
        };
        dbContext.Roles.Add(adminRole);
        await dbContext.SaveChangesAsync();
    }

    var existingAdmin = await dbContext.Users
        .Include(u => u.Roles)
        .FirstOrDefaultAsync(u => u.UserName == adminUserName);

    var passwordService = new PasswordService();

    if (existingAdmin == null)
    {
        var userId = Guid.NewGuid().ToString();
        var passwordHash = passwordService.ComputeHash(adminPassword, userId);

        var adminUser = new User
        {
            UserID = userId,
            UserName = adminUserName,
            PasswordHash = passwordHash,
            EntryDate = DateTime.Now,
            Roles = new List<Role> { adminRole }
        };

        dbContext.Users.Add(adminUser);
        await dbContext.SaveChangesAsync();
        return;
    }

    var expectedHash = passwordService.ComputeHash(adminPassword, existingAdmin.UserID);
    if (!string.Equals(existingAdmin.PasswordHash, expectedHash, StringComparison.Ordinal))
    {
        existingAdmin.PasswordHash = expectedHash;
        await dbContext.SaveChangesAsync();
    }

    if (!existingAdmin.Roles.Any(r => r.RoleName == adminRoleName))
    {
        existingAdmin.Roles.Add(adminRole);
        await dbContext.SaveChangesAsync();
    }
}
