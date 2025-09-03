using PustokApp.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokApp.Models
{
    public class Book : AuditEntity
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 9999.99, ErrorMessage = "Price must be between 0 and 9999.99")]
        public decimal Price { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
        public decimal DiscountPercentage { get; set; }
        
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public bool InStock { get; set; } = true;
        
        [StringLength(50, ErrorMessage = "Code cannot be longer than 50 characters")]
        public string? Code { get; set; }
        
        [Required(ErrorMessage = "Please select an author")]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        
        [Required(ErrorMessage = "Please select a genre")]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        
        public List<BookImage>? BookImages { get; set; }
        
        public string? MainImageUrl { get; set; }
        public string? HoverImageUrl { get; set; }
        
        public List<BookTag>? BookTags { get; set; }
    }
}
