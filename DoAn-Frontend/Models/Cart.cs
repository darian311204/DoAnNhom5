namespace DoAn_Frontend.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
        public Product? Product { get; set; }
    }
}
