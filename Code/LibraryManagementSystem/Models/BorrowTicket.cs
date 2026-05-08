using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class BorrowTicket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Khách hàng")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [Required]
        [Display(Name = "Ngày mượn")]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Hạn trả")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Ngày trả thực tế")]
        public DateTime? ReturnDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Borrowed"; // Borrowed, Returned, Overdue

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "Ngày tạo phiếu")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Nhân viên tạo phiếu")]
        public int CreatedByEmployeeId { get; set; }

        [ForeignKey("CreatedByEmployeeId")]
        public virtual Employee? CreatedByEmployee { get; set; }

        [Display(Name = "Nhân viên nhận sách")]
        public int? ReceivedByEmployeeId { get; set; }

        [ForeignKey("ReceivedByEmployeeId")]
        public virtual Employee? ReceivedByEmployee { get; set; }

        public virtual ICollection<BorrowTicketDetail>? TicketDetails { get; set; }
    }
}
