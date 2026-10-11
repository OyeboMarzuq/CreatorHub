namespace CreatorHub.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        // Always stored trimmed + lower-case.
        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Region { get; set; } = null!;
        public string Gender { get; set; } = null!;

        // "User" or "Admin"
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        // Brute-force protection
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndUtc { get; set; }

        // Forgot password: only the SHA-256 hash of the emailed token is stored.
        public string? PasswordResetTokenHash { get; set; }
        public DateTime? PasswordResetExpiresUtc { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
