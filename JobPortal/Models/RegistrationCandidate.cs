using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class RegistrationCandidate
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        // Thông tin Candidates
        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(1000)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
    }
}
