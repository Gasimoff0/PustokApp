using PustokApp.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models
{
    public class Slider : AuditEntity
    {
        public string? ImageUrl { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
        public string Title { get; set; }
        
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Button text is required")]
        [StringLength(100, ErrorMessage = "Button text cannot be longer than 100 characters")]
        public string ButtonText { get; set; }
        
        [StringLength(200, ErrorMessage = "Button link cannot be longer than 200 characters")]
        public string? ButtonLink { get; set; }
        
        [Range(0, 999, ErrorMessage = "Order must be between 0 and 999")]
        public int Order { get; set; }
    }
}
