using JonasWahle.de.Data;
using JonasWahle.de.Domain.Interfaces;
using JonasWahle.de.Domain.Services;
using JonasWahle.de.Domain.Services.Auth;
using JonasWahle.de.UI.Components;
using JonasWahle.de.UI.Interfaces;
using JonasWahle.de.UI.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Expiration = TimeSpan.Zero;
});

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Extensions.Http", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File($"{AppContext.BaseDirectory}/logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add EF Core
builder.Services.AddDbContextFactory<ApplicationContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add base services
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controllers for API endpoints
builder.Services.AddControllers();

// Add application services
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IClipboardService, ClipboardService>();
builder.Services.AddScoped<ISnackbarService, SnackbarService>();
builder.Services.AddScoped<IDownloadItemService, DownloadItemService>();
builder.Services.AddScoped<ISmtpSettingService, SmtpSettingService>();

// Add authentication services
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();

WebApplication app = builder.Build();

Log.Logger.Debug("--------- Application Start ---------");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .DisableAntiforgery()
    .AddInteractiveServerRenderMode();

// Map API controllers
app.MapControllers();

// Create DB if it doesnt exist
using (IServiceScope scope = app.Services.CreateScope())
{
    IDbContextFactory<ApplicationContext> dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationContext>>();
    using ApplicationContext dbContext = await dbFactory.CreateDbContextAsync();
    dbContext.Database.Migrate();
}

app.Run();
