using CreatorHub.Application.DTOs.Auth;

namespace CreatorHub.Application.Auth;

public class AuthResult
{
    public bool Succeeded { get; private init; }
    public string? Error { get; private init; }
    public AuthUserDto? User { get; private init; }

    public static AuthResult Ok(AuthUserDto? user = null) => new() { Succeeded = true, User = user };
    public static AuthResult Fail(string error) => new() { Succeeded = false, Error = error };
}
