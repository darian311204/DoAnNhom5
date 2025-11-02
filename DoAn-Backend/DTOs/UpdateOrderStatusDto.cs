using System.ComponentModel.DataAnnotations;

namespace DoAn_Backend.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}

