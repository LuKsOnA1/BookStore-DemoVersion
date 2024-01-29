using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BookRazorVariation_temp.Models
{
    public class Category
    {
        // This is Category Model where we define props for Database and write some validation and use definitions...
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string? Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order Must Be Between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
