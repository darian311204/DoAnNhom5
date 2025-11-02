using System.ComponentModel.DataAnnotations;

namespace DoAn_Backend.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

