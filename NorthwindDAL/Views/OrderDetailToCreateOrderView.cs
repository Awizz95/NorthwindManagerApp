namespace NorthwindDAL.Views
{
    public class OrderDetailToCreateOrderView
    {
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public double Quantity { get; set; }
        public double Discount { get; set; }
    }
}
