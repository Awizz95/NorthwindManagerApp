using NorthwindDAL.Models;

namespace NorthwindDAL.Views
{
    public class GetAllOrderDetailsView
    {
        public int OrderID { get; set; }
        public required string CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public List<OrderDetailWithProductInfoView> OrderDetails { get; set; } = new ();
        public OrderStatus Status { get; set; }
    }
}
