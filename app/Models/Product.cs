using System.ComponentModel.DataAnnotations;

namespace app.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    // Navigation property for many-to-many relationship with Tags
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}
