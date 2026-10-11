using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CreatorHub.Application.Auth;
using CreatorHub.Application.DTOs.Auth;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Services;

public class AuthService : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromHours(1);

    private const string InvalidCredentials = "Invalid email or password.";
    private const string InvalidResetLink = "This reset link is invalid or has expired. Please request a new one.";

    // Well-formed hash of nothing, verified against when the email is unknown so
    // "unknown email" and "wrong password" take the same time.
    private const string DummyHash =
        "v1.600000.AAAAAAAAAAAAAAAAAAAAAA==.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

    private static readonly Regex EmailRegex = new(AuthRules.EmailPattern, RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new(AuthRules.PasswordPattern, RegexOptions.Compiled);

    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IEmailSender _email;

    public AuthService(IUserRepository users, IPasswordHasher hasher, IEmailSender email)
    {
        _users = users;
        _hasher = hasher;
        _email = email;
    }

    // ───────────────────────── Sign up ─────────────────────────

    public async Task<AuthResult> RegisterAsync(RegisterDto request)
    {
        var email = Normalize(request.Email);
        var firstName = request.FirstName?.Trim() ?? "";
        var lastName = request.LastName?.Trim() ?? "";

        if (firstName.Length == 0 || lastName.Length == 0)
            return AuthResult.Fail("First name and last name are required.");

        if (!EmailRegex.IsMatch(email))
            return AuthResult.Fail(AuthRules.EmailMessage);

        if (!AuthRules.Regions.Contains(request.Region))
            return AuthResult.Fail("Please choose a valid region.");

        if (!AuthRules.Genders.Contains(request.Gender))
            return AuthResult.Fail("Please choose a valid gender.");

        if (!IsValidPassword(request.Password))
            return AuthResult.Fail(AuthRules.PasswordMessage);

        if (await _users.ExistsByEmailAsync(email))
            return AuthResult.Fail("An account with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Region = request.Region,
            Gender = request.Gender,
            PasswordHash = _hasher.Hash(request.Password)
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        return AuthResult.Ok(MapToDto(user));
    }

    // ───────────────────────── Login ─────────────────────────

    public async Task<AuthResult> LoginAsync(LoginDto request)
    {
        var email = Normalize(request.Email);
        var password = request.Password ?? "";

        var user = await _users.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            _hasher.Verify(DummyHash, password);
            return AuthResult.Fail(InvalidCredentials);
        }

        if (user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value > DateTime.UtcNow)
        {
            return AuthResult.Fail("Too many failed attempts. Please try again in a few minutes.");
        }

        if (!_hasher.Verify(user.PasswordHash, password))
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= MaxFailedAttempts)
            {
                user.LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
                user.FailedLoginCount = 0;
            }

            await _users.SaveChangesAsync();
            return AuthResult.Fail(InvalidCredentials);
        }

        user.FailedLoginCount = 0;
        user.LockoutEndUtc = null;
        await _users.SaveChangesAsync();

        return AuthResult.Ok(MapToDto(user));
    }

    // ───────────────────────── Forgot password ─────────────────────────

    public async Task RequestPasswordResetAsync(string email, Func<string, string, string> buildResetLink)
    {
        email = Normalize(email);

        var user = await _users.GetByEmailAsync(email);
        if (user == null || !user.IsActive) return;

        var token = GenerateToken();

        user.PasswordResetTokenHash = HashToken(token);
        user.PasswordResetExpiresUtc = DateTime.UtcNow.Add(ResetTokenLifetime);
        user.UpdatedAt = DateTime.UtcNow;
        await _users.SaveChangesAsync();

        var link = WebUtility.HtmlEncode(buildResetLink(user.Email, token));
        var name = WebUtility.HtmlEncode(user.FirstName);

        var body =
            $"<p>Hi {name},</p>" +
            "<p>We received a request to reset your CreatorHub password. " +
            "Click the link below to choose a new one. It expires in 1 hour.</p>" +
            $"<p><a href=\"{link}\">Reset my password</a></p>" +
            "<p>If you didn't ask for this, you can safely ignore this email.</p>";

        await _email.SendAsync(user.Email, "Reset your CreatorHub password", body);
    }

    public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto request)
    {
        if (!IsValidPassword(request.NewPassword))
            return AuthResult.Fail(AuthRules.PasswordMessage);

        var user = await _users.GetByEmailAsync(Normalize(request.Email));

        if (user == null
            || user.PasswordResetTokenHash == null
            || user.PasswordResetExpiresUtc == null
            || user.PasswordResetExpiresUtc < DateTime.UtcNow
            || string.IsNullOrEmpty(request.Token)
            || !TokensMatch(user.PasswordResetTokenHash, HashToken(request.Token)))
        {
            return AuthResult.Fail(InvalidResetLink);
        }

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetExpiresUtc = null;
        user.FailedLoginCount = 0;
        user.LockoutEndUtc = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _users.SaveChangesAsync();
        return AuthResult.Ok();
    }

    // ───────────────────────── Change password ─────────────────────────

    public async Task<AuthResult> ChangePasswordAsync(Guid userId, ChangePasswordDto request)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            return AuthResult.Fail("Account not found.");

        if (!_hasher.Verify(user.PasswordHash, request.CurrentPassword ?? ""))
            return AuthResult.Fail("Your current password is incorrect.");

        if (!IsValidPassword(request.NewPassword))
            return AuthResult.Fail(AuthRules.PasswordMessage);

        if (_hasher.Verify(user.PasswordHash, request.NewPassword))
            return AuthResult.Fail("Your new password must be different from the current one.");

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        // A pending reset link must not outlive a password change.
        user.PasswordResetTokenHash = null;
        user.PasswordResetExpiresUtc = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _users.SaveChangesAsync();
        return AuthResult.Ok();
    }

    // ───────────────────────── Helpers ─────────────────────────

    private static string Normalize(string? email) => (email ?? "").Trim().ToLowerInvariant();

    private static bool IsValidPassword(string? password) =>
        !string.IsNullOrEmpty(password) && PasswordRegex.IsMatch(password);

    private static string GenerateToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static bool TokensMatch(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));

    private static AuthUserDto MapToDto(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        Region = u.Region,
        Gender = u.Gender,
        Role = u.Role
    };
}
