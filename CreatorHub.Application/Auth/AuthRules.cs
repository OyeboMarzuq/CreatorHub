namespace CreatorHub.Application.Auth;

/// <summary>Single source of truth for sign-up rules (used by the service AND the MVC view models).</summary>
public static class AuthRules
{
    // Case-insensitive written out by hand so the same pattern also works in the browser (jquery-validate).
    public const string EmailPattern = @"^[^@\s]+@[gG][mM][aA][iI][lL]\.[cC][oO][mM]$";
    public const string EmailMessage = "Only @gmail.com addresses are allowed.";

    // At least 8 characters, with at least one letter and one number.
    public const string PasswordPattern = @"^(?=.*[A-Za-z])(?=.*\d).{8,100}$";
    public const string PasswordMessage = "Password must be 8-100 characters and include a letter and a number.";

    public static readonly string[] Regions =
    {
        "Africa", "Asia", "Europe", "North America", "South America", "Oceania"
    };

    public static readonly string[] Genders =
    {
        "Male", "Female", "Other"
    };
}
