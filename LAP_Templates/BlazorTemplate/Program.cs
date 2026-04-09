using BlazorTemplate.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using BLDAL;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // alte Provider entfernen
    logging.AddConsole();     // Konsole
    logging.AddDebug();       // Visual Studio Debug
    logging.SetMinimumLevel(LogLevel.Information); // Level
});

var dataProtectionPath = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
    ? "/app/keys"
    : Path.Combine(builder.Environment.ContentRootPath, ".keys");
Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("BlazorTemplate");

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("MyDatabase")
    ?? throw new InvalidOperationException("Es wurde kein ConnectionString für die Datenbank konfiguriert.");

// DbContextFactory registrieren -- added
builder.Services.AddDbContextFactory<AppDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Für die abfrage der Client IP über HTTPContext
builder.Services.AddHttpContextAccessor();

//Auth state weitergabe und einfügen der Unit of work
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<BLDAL.UnitOfWork>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDBContext>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("StartupMigration");
    const int maxRetries = 10;
    var migrated = false;

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await using var dbContext = await dbFactory.CreateDbContextAsync();
            logger.LogInformation("Wende EF-Migrationen an ({Attempt}/{MaxRetries}) ...", attempt, maxRetries);
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Datenbank ist aktuell.");
            migrated = true;
            break;
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            logger.LogWarning(ex, "Datenbank noch nicht bereit. Neuer Versuch in 5 Sekunden ...");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    if (!migrated)
    {
        throw new InvalidOperationException("Die Datenbankmigration konnte nach mehreren Versuchen nicht angewendet werden.");
    }
}

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
