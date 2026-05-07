using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thể loại không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên thể loại")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Book>? Books { get; set; }
    }
}
