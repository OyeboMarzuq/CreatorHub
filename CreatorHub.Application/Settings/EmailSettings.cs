namespace CreatorHub.Application.Settings;

public class EmailSettings
{
    // Leave Host empty in development: the reset link is written to the log instead of being emailed.
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromEmail { get; set; } = "no-reply@example.com";
    public string FromName { get; set; } = "CreatorHub";
}
