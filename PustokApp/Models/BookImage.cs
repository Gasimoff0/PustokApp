using PustokApp.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models
{
    public class BookImage:BaseEntity
    {
        [Required]
        public string ImageUrl { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
