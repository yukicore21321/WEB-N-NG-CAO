using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống")]
        [StringLength(50)]
        [Display(Name = "Tài khoản")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(200)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [StringLength(50)]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Staff"; // Admin, Staff

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties for tickets created/received by this employee
        public virtual ICollection<BorrowTicket>? CreatedTickets { get; set; }
        public virtual ICollection<BorrowTicket>? ReceivedTickets { get; set; }
    }
}
