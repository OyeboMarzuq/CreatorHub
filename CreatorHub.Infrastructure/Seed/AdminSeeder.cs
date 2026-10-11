using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;
using CreatorHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CreatorHub.Infrastructure.Seed;

public static class AdminSeeder
{
    /// <summary>
    /// Creates the admin account on startup if it does not exist yet.
    /// If the email already exists it is promoted to Admin (its password is NOT changed).
    /// Does nothing when email/password are not configured.
    /// </summary>
    public static async Task SeedAsync(
        IServiceProvider services, string? email, string? password, string? firstName, string? lastName)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeeder");

        email = (email ?? "").Trim().ToLowerInvariant();

        if (email.Length == 0 || string.IsNullOrEmpty(password))
        {
            logger.LogInformation("AdminSeed:Email / AdminSeed:Password not configured - skipping admin seeding.");
            return;
        }

        if (password.Length < 8)
        {
            logger.LogWarning("AdminSeed:Password must be at least 8 characters - skipping admin seeding.");
            return;
        }

        try
        {
            var db = services.GetRequiredService<CreatorHubDbContext>();
            var hasher = services.GetRequiredService<IPasswordHasher>();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                db.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = string.IsNullOrWhiteSpace(firstName) ? "Admin" : firstName.Trim(),
                    LastName = string.IsNullOrWhiteSpace(lastName) ? "CreatorHub" : lastName.Trim(),
                    Email = email,
                    PasswordHash = hasher.Hash(password),
                    Region = "Africa",
                    Gender = "Other",
                    Role = "Admin"
                });

                logger.LogInformation("Seeded admin account {Email}.", email);
            }
            else if (user.Role != "Admin" || !user.IsActive)
            {
                user.Role = "Admin";
                user.IsActive = true;
                logger.LogInformation("Promoted existing account {Email} to Admin.", email);
            }
            else
            {
                return;
            }

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Could not seed the admin account. Have the database migrations been applied?");
        }
    }
}
