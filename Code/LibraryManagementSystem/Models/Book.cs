using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tác giả không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tác giả")]
        public string Author { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Nhà xuất bản")]
        public string? Publisher { get; set; }

        [Display(Name = "Năm xuất bản")]
        public int? PublishYear { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Số lượng còn lại")]
        public int AvailableQuantity { get; set; }

        [Display(Name = "Ảnh bìa URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Thể loại")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<BorrowTicketDetail>? BorrowDetails { get; set; }
    }
}
