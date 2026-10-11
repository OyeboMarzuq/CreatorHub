namespace CreatorHub.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string storedHash, string password);
}
