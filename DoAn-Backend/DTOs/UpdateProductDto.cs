using System.ComponentModel.DataAnnotations;

namespace DoAn_Backend.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ImageURL { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

