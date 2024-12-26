using NorthwindBL.Interfaces;
using NorthwindDAL.Interfaces;
using NorthwindDAL.Models;
using NorthwindDAL.Views;

namespace NorthwindBL.Services
{
    public class AppLogic : IAppLogic
    {
        private readonly IRepository _repository;

        public AppLogic(IRepository repository)
        {
            _repository = repository;
        }

        public int CreateOrder(OrderCreateView order, IEnumerable<OrderDetailToCreateOrderView> orderDetails)
        {
            return _repository.CreateOrder(order, orderDetails);
        }

        public void DeleteOrder(int orderId)
        {
            _repository.DeleteOrder(orderId);
        }

        public GetAllOrderDetailsView? GetAllOrderDetails(int orderId)
        {
            return _repository.GetAllOrderDetails(orderId);
        }

        public IEnumerable<OrderWithStatusView> GetOrders()
        {
            return _repository.GetOrders();
        }

        public OrderStatisticsView GetOrderStatistics(string customerID, int orderID)
        {
            return _repository.GetOrderStatistics(customerID, orderID);
        }

        public void MarkOrderAsCompleted(int orderId)
        {
            _repository.MarkOrderAsCompleted(orderId);
        }

        public void MarkOrderAsInProgress(int orderId)
        {
            _repository.MarkOrderAsInProgress(orderId);
        }

        public void UpdateOrder(Order order)
        {
            _repository.UpdateOrder(order);
        }
    }
}
