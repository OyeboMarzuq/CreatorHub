using CreatorHub.Application.Auth;
using CreatorHub.Application.DTOs.Auth;

namespace CreatorHub.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto request);

    Task<AuthResult> LoginAsync(LoginDto request);

    /// <summary>
    /// Always completes silently (no hint whether the email exists).
    /// buildResetLink receives (email, rawToken) and returns the absolute URL to email.
    /// </summary>
    Task RequestPasswordResetAsync(string email, Func<string, string, string> buildResetLink);

    Task<AuthResult> ResetPasswordAsync(ResetPasswordDto request);

    Task<AuthResult> ChangePasswordAsync(Guid userId, ChangePasswordDto request);
}
