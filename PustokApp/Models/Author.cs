using PustokApp.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models
{
    public class Author: BaseEntity
    {
        [Required]
        [StringLength(50,ErrorMessage ="Name cannot be longer than 50 characters")]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
