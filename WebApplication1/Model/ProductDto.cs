using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;



    }
}
