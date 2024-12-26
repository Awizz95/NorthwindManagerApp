using NorthwindDAL.Models;
using NorthwindDAL.Views;

namespace NorthwindBL.Interfaces
{
    public interface IAppLogic
    {
        IEnumerable<OrderWithStatusView> GetOrders();
        GetAllOrderDetailsView? GetAllOrderDetails(int orderId);
        int CreateOrder(OrderCreateView order, IEnumerable<OrderDetailToCreateOrderView> orderDetails);
        void UpdateOrder(Order order);
        void DeleteOrder(int orderId);
        void MarkOrderAsInProgress(int orderId);
        void MarkOrderAsCompleted(int orderId);
        OrderStatisticsView GetOrderStatistics(string customerID, int orderID);
    }
}
