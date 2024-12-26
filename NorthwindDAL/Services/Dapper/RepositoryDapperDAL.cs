using Dapper;
using NorthwindDAL.Infrastructure;
using NorthwindDAL.Interfaces;
using NorthwindDAL.Models;
using NorthwindDAL.Views;
using NorthwindDAL.Views.StoredProceduresRes;
using System.Data;
using System.Data.Common;

namespace NorthwindDAL.Services.Dapper
{
    public class RepositoryDapperDAL : IRepository
    {
        private readonly string connectionString;
        private readonly DbProviderFactory factory;
        private static OrderStatus GetOrderStatus(DateTime? orderDate, DateTime? shippedDate)
        {
            if (orderDate is null)
            {
                return OrderStatus.New;
            }

            if (shippedDate is null)
            {
                return OrderStatus.InProgress;
            }

            return OrderStatus.Completed;
        }
        private DbConnection CreateAndOpenConnection()
        {
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();

            return connection;
        }
        public RepositoryDapperDAL(string connString, string providerName)
        {
            connectionString = connString;
            DbProviderFactories.RegisterFactory(providerName, Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            factory = DbProviderFactories.GetFactory(providerName);
        }

        public int CreateOrder(OrderCreateView order, IEnumerable<OrderDetailToCreateOrderView> orderDetails)
        {
            using var connection = CreateAndOpenConnection();

            var orderSql = "INSERT INTO Orders " +
                "(CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)" +
                "VALUES" +
                "(@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry);" +
                "SELECT SCOPE_IDENTITY()";

            var orderId = connection.ExecuteScalar<int>(orderSql, order);

            foreach (var detail in orderDetails)
            {
                var detailSql = @"INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)
                    VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";

                connection.Execute(detailSql, new { OrderID = orderId, detail.ProductID, detail.UnitPrice, detail.Quantity, detail.Discount });
            }

            return orderId;
        }

        public void DeleteOrder(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var statusSql = "SELECT OrderDate, ShippedDate FROM Orders WHERE OrderID = @orderId";

            var order = connection.QuerySingleOrDefault<Order>(statusSql, new { orderId });

            if (order is null)
            {
                throw new ArgumentException(message: ExtensionMessage.ORDER_NOT_FOUND);
            }

            var status = GetOrderStatus(order.OrderDate, order.ShippedDate);

            if (status == OrderStatus.New || status == OrderStatus.InProgress)
            {
                var odSql =  "DELETE FROM [Order Details] WHERE OrderID = @OrderID";

                connection.Execute(odSql, new { orderId });

                var oSql = "DELETE FROM Orders WHERE OrderID = @orderID";

                connection.Execute(oSql, new { orderId });
            }
            else if (status == OrderStatus.Completed)
            {
                throw new InvalidOperationException(message: ExtensionMessage.CANNOT_DELETE_COMPLETED);
            }
            else
            {
                throw new InvalidOperationException(message: ExtensionMessage.UNKNOWN_STATUS);
            }
        }

        public GetAllOrderDetailsView? GetAllOrderDetails(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var orderSql = "SELECT * FROM Orders WHERE OrderID = @orderID";

            var order = connection.QuerySingleOrDefault<GetAllOrderDetailsView>(orderSql, new { orderId });

            if (order is null)
            {
                return null;
            }

            order.Status = GetOrderStatus(order.OrderDate, order.ShippedDate);

            var detailsSql = "SELECT od.ProductID, p.ProductName, od.UnitPrice, od.Quantity, od.Discount " +
                "FROM [Order Details] od " +
                "JOIN Products p ON od.ProductID = p.ProductID " +
                "WHERE od.OrderID = @orderID";

            order.OrderDetails = connection.Query<OrderDetailWithProductInfoView>(detailsSql, new { orderId }).ToList();

            return order;
        }

        public IEnumerable<OrderWithStatusView> GetOrders()
        {
            using var connection = CreateAndOpenConnection();

            var sql = "SELECT * FROM Orders";
            var orders = connection.Query<OrderWithStatusView>(sql).ToList();

            foreach (var order in orders)
            {
                order.Status = GetOrderStatus(order.OrderDate, order.ShippedDate);
            }

            return orders;
        }

        public OrderStatisticsView GetOrderStatistics(string customerID, int orderID)
        {
            using var connection = CreateAndOpenConnection();

            var statistics = new OrderStatisticsView();

            var cohSql = "CustOrderHist";
            statistics.CustOrderHistRes = connection.Query<CustOrderHist>(cohSql, new { customerID },commandType: CommandType.StoredProcedure).ToList();

            var codSql = "CustOrdersDetail";
            statistics.CustOrdersDetailRes = connection.Query<CustOrdersDetail>(codSql, new { orderID }, commandType: CommandType.StoredProcedure).ToList();

            return statistics;
        }

        public void MarkOrderAsCompleted(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var sql = "UPDATE Orders SET ShippedDate = @ShippedDate WHERE OrderID = @OrderID";

            connection.Execute(sql, new { ShippedDate = DateTime.Now, orderId });
        }

        public void MarkOrderAsInProgress(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var sql = "UPDATE Orders SET OrderDate = @OrderDate WHERE OrderID = @orderID";

            connection.Execute(sql, new { OrderDate = DateTime.Now, orderId });
        }

        public void UpdateOrder(Order order)
        {
            using var connection = CreateAndOpenConnection();

            var statusSql = "SELECT * FROM Orders WHERE OrderID = @orderId";

            var oldOrder = connection.QuerySingleOrDefault<OrderWithStatusView>(statusSql, new { order.OrderID });

            if (oldOrder is null)
            {
                throw new ArgumentException(message: ExtensionMessage.ORDER_NOT_FOUND);
            }

            var oldOrderStatus = GetOrderStatus(oldOrder.OrderDate, oldOrder.ShippedDate);

            if (oldOrderStatus == OrderStatus.New)
            {
                var oSql = "UPDATE Orders " +
                    "SET " +
                    "CustomerID = @CustomerID," +
                    "EmployeeID = @EmployeeID," +
                    "RequiredDate = @RequiredDate," +
                    "ShipVia = @ShipVia," +
                    "Freight = @Freight," +
                    "ShipName = @ShipName," +
                    "ShipAddress = @ShipAddress," +
                    "ShipCity = @ShipCity," +
                    "ShipRegion = @ShipRegion," +
                    "ShipPostalCode = @ShipPostalCode," +
                    "ShipCountry = @ShipCountry " +
                    "WHERE OrderID = @OrderID";

                connection.Execute(oSql, order);

                var odSql = "DELETE FROM [Order Details] WHERE OrderID = @OrderID";

                connection.Execute(odSql, new { order.OrderID });

                foreach (var detail in order.OrderDetails)
                {
                    var detailSql = @"INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)
                    VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";

                    connection.Execute(detailSql, new { OrderID = order.OrderID, detail.ProductID, detail.UnitPrice, detail.Quantity, detail.Discount });
                }
            }
            else if (oldOrderStatus == OrderStatus.InProgress || oldOrderStatus == OrderStatus.Completed)
            {
                throw new InvalidOperationException(message: ExtensionMessage.CANNOT_UPDATE_INPROGRESS_OR_COMPLETED);
            }
            else
            {
                throw new InvalidOperationException(message: ExtensionMessage.UNKNOWN_STATUS);
            }
        }
    }
}
