using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class BorrowTicketDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BorrowTicketId { get; set; }

        [ForeignKey("BorrowTicketId")]
        public virtual BorrowTicket? BorrowTicket { get; set; }

        [Required]
        [Display(Name = "Sách")]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book? Book { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
    }
}
