using NorthwindDAL.Models;
using NorthwindDAL.Views;

namespace NorthwindDAL.Interfaces
{
    public interface IRepository
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
