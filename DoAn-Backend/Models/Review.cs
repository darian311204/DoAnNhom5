using System.ComponentModel.DataAnnotations;

namespace DoAn_Backend.Models
{
    public class Review
    {
        public int ReviewID { get; set; }

        public int ProductID { get; set; }

        public int UserID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
