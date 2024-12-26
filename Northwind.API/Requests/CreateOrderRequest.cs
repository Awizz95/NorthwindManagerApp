using NorthwindDAL.Views;

namespace Northwind.API.Requests
{
    public class CreateOrderRequest
    {
        public OrderCreateView Order { get; set; }
        public IEnumerable<OrderDetailToCreateOrderView> OrderDetails { get; set; }
    }
}
