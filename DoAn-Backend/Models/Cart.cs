using System.ComponentModel.DataAnnotations;

namespace DoAn_Backend.Models
{
    public class Cart
    {
        public int CartID { get; set; }

        public int UserID { get; set; }

        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
