namespace NorthwindDAL.Views
{
    public class OrderDetailWithProductInfoView
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? Quantity { get; set; }
        public float? Discount { get; set; }
    }
}
