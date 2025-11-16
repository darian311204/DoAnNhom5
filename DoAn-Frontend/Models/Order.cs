namespace DoAn_Frontend.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? CancelReason { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Phone { get; set; }
        public string? RecipientName { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Product? Product { get; set; }
    }
}
