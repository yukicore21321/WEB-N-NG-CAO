using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public enum BorrowStatus
    {
        [Display(Name = "Đang mượn")]
        Borrowed,
        [Display(Name = "Đã trả")]
        Returned,
        [Display(Name = "Quá hạn")]
        Overdue
    }

    public class BorrowRecord
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book? Book { get; set; }

        [Required]
        [Display(Name = "Ngày mượn")]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Hạn trả")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Ngày trả thực tế")]
        public DateTime? ReturnDate { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;
    }
}
