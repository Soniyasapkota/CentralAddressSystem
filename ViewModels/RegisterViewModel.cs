using System.ComponentModel.DataAnnotations;

namespace CentralAddressSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[A-Za-z\s'-]+$", ErrorMessage = "First name must contain letters, spaces, hyphens, or apostrophes only.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[A-Za-z\s'-]+$", ErrorMessage = "Last name must contain letters, spaces, hyphens, or apostrophes only.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[^\d][\w]*$", ErrorMessage = "Username must not start with a number.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}