using System.ComponentModel.DataAnnotations;

namespace app.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    // Navigation property for many-to-many relationship with Products
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}
