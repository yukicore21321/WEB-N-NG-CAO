using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Tác giả")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "ISBN")]
        public string? ISBN { get; set; }

        [Required]
        [Display(Name = "Tổng số bản")]
        public int TotalCopies { get; set; }

        [Required]
        [Display(Name = "Số bản hiện có")]
        public int AvailableCopies { get; set; }

        [Display(Name = "Ảnh bìa")]
        public string? ImagePath { get; set; }

        [Display(Name = "File tài liệu số (PDF)")]
        public string? FilePath { get; set; }

        [Required]
        [Display(Name = "Thể loại")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<BorrowRecord>? BorrowRecords { get; set; }
    }
}
