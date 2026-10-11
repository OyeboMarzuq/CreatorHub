using System.ComponentModel.DataAnnotations;
using CreatorHub.Application.Auth;

namespace CreatorHub.Web.Models.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "First name is required."), StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Last name is required."), StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Email is required."), StringLength(254)]
    [RegularExpression(AuthRules.EmailPattern, ErrorMessage = AuthRules.EmailMessage)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please choose your region.")]
    public string Region { get; set; } = "";

    [Required(ErrorMessage = "Please choose your gender.")]
    public string Gender { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(AuthRules.PasswordPattern, ErrorMessage = AuthRules.PasswordMessage)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = "";
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Keep me signed in")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(AuthRules.EmailPattern, ErrorMessage = AuthRules.EmailMessage)]
    public string Email { get; set; } = "";
}

public class ResetPasswordViewModel
{
    [Required] public string Email { get; set; } = "";
    [Required] public string Token { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(AuthRules.PasswordPattern, ErrorMessage = AuthRules.PasswordMessage)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = "";

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    public string ConfirmPassword { get; set; } = "";
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Enter your current password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(AuthRules.PasswordPattern, ErrorMessage = AuthRules.PasswordMessage)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = "";

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    public string ConfirmPassword { get; set; } = "";
}
