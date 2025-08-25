using BlazorTemplate.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using BLDAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
});

// DbContext registrieren und Connection String aus appsettings.json laden
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase"))
);

// Für die abfrage der Client IP über HTTPContext
builder.Services.AddHttpContextAccessor();

//Auth state weitergabe und einfügen der Unit of work
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<BLDAL.UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
