namespace NorthwindDAL.Views.StoredProceduresRes
{
    public class CustOrdersDetail
    {
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public decimal ExtendedPrice { get; set; }
    }
}
