using CreatorHub.Application.Importers;
using CreatorHub.Application.Interfaces.Importers;
using CreatorHub.Application.Settings;
using CreatorHub.Infrastructure.Data;
using CreatorHub.Infrastructure.DependencyInjection;
using CreatorHub.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CreatorHubDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddInfrastructure();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "CreatorHub.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });

builder.Services.AddHttpClient<IProfileUrlImporter, ProfileUrlImporter>();
builder.Services.AddHttpClient<ICreatorImporter, CreatorImporter>();

var app = builder.Build();

// Seed the admin account (credentials come from configuration / user-secrets, never from code).
using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.SeedAsync(
        scope.ServiceProvider,
        app.Configuration["AdminSeed:Email"],
        app.Configuration["AdminSeed:Password"],
        app.Configuration["AdminSeed:FirstName"],
        app.Configuration["AdminSeed:LastName"]);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
