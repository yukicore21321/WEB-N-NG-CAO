using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        public bool MustChangePassword { get; set; } = false;
        public DateTime? LastPasswordChange { get; set; }

        public virtual ICollection<BorrowRecord>? BorrowRecords { get; set; }
    }
}
