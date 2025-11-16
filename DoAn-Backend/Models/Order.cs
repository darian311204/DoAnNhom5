using System.ComponentModel.DataAnnotations;

namespace DoAn_Backend.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Shipping, Shipped, Cancelled

        [StringLength(200)]
        public string? CancelReason { get; set; }

        [StringLength(200)]
        public string? ShippingAddress { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? RecipientName { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
