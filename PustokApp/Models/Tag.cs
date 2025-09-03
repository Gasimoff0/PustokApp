using PustokApp.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models
{
    public class Tag : BaseEntity
    {
        [Required(ErrorMessage = "Tag name is required")]
        [StringLength(50, ErrorMessage = "Tag name cannot be longer than 50 characters")]
        public string Name { get; set; }
        
        public List<BookTag>? BookTags { get; set; }
    }

    public class BookTag
    {
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
