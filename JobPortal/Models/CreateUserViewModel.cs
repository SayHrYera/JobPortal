namespace JobPortal.Models
{
    public class CreateUserViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string NumberPhone { get; set; }
        public int RoleId { get; set; } // RoleId kiểu int
        public string RoleName { get; set; } // RoleName thêm mới

        // Các thông tin Employer
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string ContactName { get; set; }

        // Các thông tin Candidate
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}
