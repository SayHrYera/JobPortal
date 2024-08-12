using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class RegistrationEmployers
    {
        [Required(ErrorMessage = "Username không được bỏ trống")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password không được bỏ trống")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password phải có độ dài từ 6-20 kí tự")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords không khớp")]
        public string ConfirmPassword { get; set; }

        // Thông tin Candidates
        [Required(ErrorMessage = "Company Name không được bỏ trống")]
        [MaxLength(400)]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Contact Name không được bỏ trống")]  
        [MaxLength(100)]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "Address không được bỏ trống")]
        [MaxLength(1000)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number không được bỏ trống")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
    }
}
