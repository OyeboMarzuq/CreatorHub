
using CreatorHub.Application.Importers;
using CreatorHub.Application.Interfaces.Importers;
using CreatorHub.Application.Settings;
using CreatorHub.Infrastructure.Data;
using CreatorHub.Infrastructure.DependencyInjection;
using CreatorHub.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Globalization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Localization: German by default, English as an alternative.
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

var supportedCultures = new[]
{
    new CultureInfo("de-DE"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("de-DE");

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // German is the default unless the visitor has selected a language.
    // A saved culture cookie takes priority.
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(
        new CookieRequestCultureProvider());
});

// PostgreSQL
builder.Services.AddDbContext<CreatorHubDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure
builder.Services.AddInfrastructure();

// Email settings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

// Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "CreatorHub.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });

// Importers
builder.Services.AddHttpClient<IProfileUrlImporter, ProfileUrlImporter>();
builder.Services.AddHttpClient<ICreatorImporter, CreatorImporter>();

var app = builder.Build();

// Development importer — existing behavior preserved.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var creatorImporter =
        scope.ServiceProvider.GetRequiredService<ICreatorImporter>();

    await creatorImporter.ImportAsync(10);
}

// Database migrations and admin seeding — existing behavior preserved.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CreatorHubDbContext>();

    try
    {
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(
            ex,
            "Database migration failed. Details: {Error}",
            ex.ToString());
    }

    await AdminSeeder.SeedAsync(
        scope.ServiceProvider,
        app.Configuration["AdminSeed:Email"],
        app.Configuration["AdminSeed:Password"],
        app.Configuration["AdminSeed:FirstName"],
        app.Configuration["AdminSeed:LastName"]);
}

// HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Apply the selected culture before routing and controller execution.
app.UseRequestLocalization();

app.UseRouting();

StripeConfiguration.ApiKey =
    builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
