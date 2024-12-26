using NorthwindDAL.Infrastructure;
using NorthwindDAL.Interfaces;
using NorthwindDAL.Models;
using NorthwindDAL.Views;
using NorthwindDAL.Views.StoredProceduresRes;
using System.Data;
using System.Data.Common;

namespace NorthwindDAL.Services.ADO.NET
{
    public class RepositoryADODAL : IRepository
    {
        private readonly string connectionString;
        private readonly DbProviderFactory factory;
        private static DbParameter CreateParameter(DbCommand command, string paramName, object? paramValue, DbType paramType)
        {
            var param = command.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue ?? DBNull.Value;
            param.DbType = paramType;

            return param;
        }
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
        public RepositoryADODAL(string connString, string providerName)
        {
            connectionString = connString;
            factory = DbProviderFactories.GetFactory(providerName);
        }

        public IEnumerable<OrderWithStatusView> GetOrders()
        {
            var orders = new List<OrderWithStatusView>();

            using var connection = CreateAndOpenConnection();

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Orders";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var order = new OrderWithStatusView
                {
                    OrderID = (int)reader["OrderID"],
                    CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : (string)reader["CustomerID"],
                    EmployeeID = reader.IsDBNull(reader.GetOrdinal("EmployeeID")) ? (int?)null : (int)reader["EmployeeID"],
                    OrderDate = reader.IsDBNull(reader.GetOrdinal("OrderDate")) ? (DateTime?)null : (DateTime)reader["OrderDate"],
                    RequiredDate = reader.IsDBNull(reader.GetOrdinal("RequiredDate")) ? (DateTime?)null : (DateTime)reader["RequiredDate"],
                    ShippedDate = reader.IsDBNull(reader.GetOrdinal("ShippedDate")) ? (DateTime?)null : (DateTime)reader["ShippedDate"],
                    ShipVia = reader.IsDBNull(reader.GetOrdinal("ShipVia")) ? (int?)null : (int)reader["ShipVia"],
                    Freight = reader.IsDBNull(reader.GetOrdinal("Freight")) ? (decimal?)null : (decimal)reader["Freight"],
                    ShipName = reader.IsDBNull(reader.GetOrdinal("ShipName")) ? null : (string)reader["ShipName"],
                    ShipAddress = reader.IsDBNull(reader.GetOrdinal("ShipAddress")) ? null : (string)reader["ShipAddress"],
                    ShipCity = reader.IsDBNull(reader.GetOrdinal("ShipCity")) ? null : (string)reader["ShipCity"],
                    ShipRegion = reader.IsDBNull(reader.GetOrdinal("ShipRegion")) ? null : (string)reader["ShipRegion"],
                    ShipPostalCode = reader.IsDBNull(reader.GetOrdinal("ShipPostalCode")) ? null : (string)reader["ShipPostalCode"],
                    ShipCountry = reader.IsDBNull(reader.GetOrdinal("ShipCountry")) ? null : (string)reader["ShipCountry"],
                    Status = GetOrderStatus(reader.IsDBNull(reader.GetOrdinal("OrderDate")) ? (DateTime?)null : (DateTime)reader["OrderDate"], reader.IsDBNull(reader.GetOrdinal("ShippedDate")) ? (DateTime?)null : (DateTime)reader["ShippedDate"])
                };

                orders.Add(order);
            }

            return orders;
        }

        public GetAllOrderDetailsView? GetAllOrderDetails(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var orderCommand = connection.CreateCommand();
            orderCommand.CommandType = CommandType.Text;
            orderCommand.CommandText = "SELECT * FROM Orders WHERE OrderID = @OrderID";

            var orderIdParam = CreateParameter(orderCommand, "@OrderID", orderId, DbType.Int32);
            orderCommand.Parameters.Add(orderIdParam);

            GetAllOrderDetailsView order = null;

            using (var reader = orderCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    order = new GetAllOrderDetailsView
                    {
                        OrderID = reader.GetInt32(0),
                        CustomerID = reader.IsDBNull("CustomerID") ? null : reader.GetString(1),
                        EmployeeID = reader.IsDBNull("EmployeeID") ? (int?)null : reader.GetInt32(2),
                        OrderDate = reader.IsDBNull("OrderDate") ? (DateTime?)null : reader.GetDateTime(3),
                        RequiredDate = reader.IsDBNull("RequiredDate") ? (DateTime?)null : reader.GetDateTime(4),
                        ShippedDate = reader.IsDBNull("ShippedDate") ? (DateTime?)null : reader.GetDateTime(5),
                        ShipVia = reader.IsDBNull("ShipVia") ? (int?)null : reader.GetInt32(6),
                        Freight = reader.IsDBNull("Freight") ? (decimal?)null : reader.GetDecimal(7),
                        ShipName = reader.IsDBNull("ShipName") ? null : reader.GetString(8),
                        ShipAddress = reader.IsDBNull("ShipAddress") ? null : reader.GetString(9),
                        ShipCity = reader.IsDBNull("ShipCity") ? null : reader.GetString(10),
                        ShipRegion = reader.IsDBNull("ShipRegion") ? null : reader.GetString(11),
                        ShipPostalCode = reader.IsDBNull("ShipPostalCode") ? null : reader.GetString(12),
                        ShipCountry = reader.IsDBNull("ShipCountry") ? null : reader.GetString(13),
                        Status = GetOrderStatus(reader.IsDBNull("OrderDate") ? (DateTime?)null : reader.GetDateTime(3), reader.IsDBNull("ShippedDate") ? (DateTime?)null : reader.GetDateTime(5))
                    };
                }

                if (order is null)
                {
                    return null;
                }
            }

            var orderDetailsCommand = connection.CreateCommand();
            orderDetailsCommand.CommandType = CommandType.Text;
            orderDetailsCommand.CommandText = $"SELECT od.ProductID, p.ProductName, od.UnitPrice, od.Quantity, od.Discount " +
                $"FROM [Order Details] od " +
                $"JOIN Products p ON od.ProductID = p.ProductID " +
                $"WHERE od.OrderID = @OrderID";

            orderIdParam = CreateParameter(orderDetailsCommand, "@OrderID", orderId, DbType.Int32);
            orderDetailsCommand.Parameters.Add(orderIdParam);

            using var orderDetailsReader = orderDetailsCommand.ExecuteReader();

            while (orderDetailsReader.Read())
            {
                var orderDetail = new OrderDetailWithProductInfoView
                {
                    ProductID = orderDetailsReader.GetInt32(0),
                    ProductName = orderDetailsReader.IsDBNull("ProductName") ? null : orderDetailsReader.GetString(1),
                    UnitPrice = orderDetailsReader.IsDBNull("UnitPrice") ? (decimal?)null : orderDetailsReader.GetDecimal(2),
                    Quantity = orderDetailsReader.IsDBNull("Quantity") ? (short?)null : orderDetailsReader.GetInt16(3),
                    Discount = orderDetailsReader.IsDBNull("Discount") ? (float?)null : orderDetailsReader.GetFloat(4),
                };

                order.OrderDetails.Add(orderDetail);
            }

            return order;
        }

        public int CreateOrder(OrderCreateView order, IEnumerable<OrderDetailToCreateOrderView> orderDetails)
        {
            using var connection = CreateAndOpenConnection();

            var orderCommand = connection.CreateCommand();
            orderCommand.CommandType = CommandType.Text;
            orderCommand.CommandText = "INSERT INTO Orders " +
                "(CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry) " +
                "VALUES " +
                "(@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry); " +
                "SELECT SCOPE_IDENTITY()";

            var customerID = CreateParameter(orderCommand, "@CustomerID", order.CustomerID, DbType.String);
            var employeeID = CreateParameter(orderCommand, "@EmployeeID", order.EmployeeID, DbType.Int32);
            var orderDate = CreateParameter(orderCommand, "@OrderDate", order.OrderDate, DbType.DateTime);
            var requiredDate = CreateParameter(orderCommand, "@RequiredDate", order.RequiredDate, DbType.DateTime);
            var shippedDate = CreateParameter(orderCommand, "@ShippedDate", order.ShippedDate, DbType.DateTime);
            var shipVia = CreateParameter(orderCommand, "@ShipVia", order.ShipVia, DbType.Int32);
            var freight = CreateParameter(orderCommand, "@Freight", order.Freight, DbType.Decimal);
            var ShipName = CreateParameter(orderCommand, "@ShipName", order.ShipName, DbType.String);
            var shipAddress = CreateParameter(orderCommand, "@ShipAddress", order.ShipAddress, DbType.String);
            var shipCity = CreateParameter(orderCommand, "@ShipCity", order.ShipCity, DbType.String);
            var shipRegion = CreateParameter(orderCommand, "@ShipRegion", order.ShipRegion, DbType.String);
            var shipPostalCode = CreateParameter(orderCommand, "@ShipPostalCode", order.ShipPostalCode, DbType.String);
            var shipCountry = CreateParameter(orderCommand, "@ShipCountry", order.ShipCountry, DbType.String);

            orderCommand.Parameters.AddRange(new[] {customerID, employeeID, orderDate, requiredDate, shippedDate, shipVia, freight, ShipName, shipAddress,
                shipCity, shipRegion, shipPostalCode, shipCountry});

            var orderId = Convert.ToInt32(orderCommand.ExecuteScalar());

            foreach (var detail in orderDetails)
            {
                var detailCommand = connection.CreateCommand();
                detailCommand.CommandType = CommandType.Text;
                detailCommand.CommandText = "INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)" +
                    "VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";

                var orderID = CreateParameter(orderCommand, "@OrderID", orderId, DbType.Int32);
                var productID = CreateParameter(orderCommand, "@ProductID", detail.ProductID, DbType.Int32);
                var unitPrice = CreateParameter(orderCommand, "@UnitPrice", detail.UnitPrice, DbType.Decimal);
                var quantity = CreateParameter(orderCommand, "@Quantity", detail.Quantity, DbType.Double);
                var discount = CreateParameter(orderCommand, "@Discount", detail.Discount, DbType.Double);

                detailCommand.Parameters.AddRange(new[] { orderID, productID, unitPrice, quantity, discount });

                detailCommand.ExecuteNonQuery();
            }

            return orderId;
        }

        public void UpdateOrder(Order order)
        {
            using var connection = CreateAndOpenConnection();

            var statusCommand = connection.CreateCommand();
            statusCommand.CommandType = CommandType.Text;
            statusCommand.CommandText = "SELECT OrderDate, ShippedDate FROM Orders WHERE OrderID = @OrderID";

            var orderIdParam = CreateParameter(statusCommand, "@OrderID", order.OrderID, DbType.Int32);
            statusCommand.Parameters.Add(orderIdParam);

            DateTime? orderDate = default;
            DateTime? shippedDate = default;

            var orderStatus = OrderStatus.None;

            using (var reader = statusCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    orderDate = reader.IsDBNull(reader.GetOrdinal("OrderDate")) ? (DateTime?)null : (DateTime)reader["OrderDate"];
                    shippedDate = reader.IsDBNull(reader.GetOrdinal("ShippedDate")) ? (DateTime?)null : (DateTime)reader["ShippedDate"];
                }
                else
                {
                    throw new ArgumentException(message: ExtensionMessage.ORDER_NOT_FOUND);
                }

                orderStatus = GetOrderStatus(orderDate, shippedDate);

            }

            if (orderStatus == OrderStatus.New)
            {
                var orderCommand = connection.CreateCommand();
                orderCommand.CommandType = CommandType.Text;
                orderCommand.CommandText = "UPDATE Orders " +
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

                orderIdParam = CreateParameter(orderCommand, "@OrderID", order.OrderID, DbType.Int32);
                orderCommand.Parameters.Add(orderIdParam);

                var customerID = CreateParameter(orderCommand, "@CustomerID", order.CustomerID, DbType.String);
                var employeeID = CreateParameter(orderCommand, "@EmployeeID", order.EmployeeID, DbType.Int32);
                var requiredDate = CreateParameter(orderCommand, "@RequiredDate", order.RequiredDate, DbType.DateTime);
                var shipVia = CreateParameter(orderCommand, "@ShipVia", order.ShipVia, DbType.Int32);
                var freight = CreateParameter(orderCommand, "@Freight", order.Freight, DbType.Decimal);
                var ShipName = CreateParameter(orderCommand, "@ShipName", order.ShipName, DbType.String);
                var shipAddress = CreateParameter(orderCommand, "@ShipAddress", order.ShipAddress, DbType.String);
                var shipCity = CreateParameter(orderCommand, "@ShipCity", order.ShipCity, DbType.String);
                var shipRegion = CreateParameter(orderCommand, "@ShipRegion", order.ShipRegion, DbType.String);
                var shipPostalCode = CreateParameter(orderCommand, "@ShipPostalCode", order.ShipPostalCode, DbType.String);
                var shipCountry = CreateParameter(orderCommand, "@ShipCountry", order.ShipCountry, DbType.String);

                orderCommand.Parameters.AddRange(new[] {customerID, employeeID, requiredDate, shipVia, freight, ShipName, shipAddress,
                        shipCity, shipRegion, shipPostalCode, shipCountry});

                orderCommand.ExecuteNonQuery();

                var detailCommand = connection.CreateCommand();
                detailCommand.CommandType = CommandType.Text;
                detailCommand.CommandText = "DELETE FROM [Order Details] WHERE OrderID = @OrderID";

                orderIdParam = CreateParameter(detailCommand, "@OrderID", order.OrderID, DbType.Int32);
                detailCommand.Parameters.Add(orderIdParam);

                detailCommand.ExecuteNonQuery();

                foreach (var detail in order.OrderDetails)
                {
                    detailCommand = connection.CreateCommand();
                    detailCommand.CommandType = CommandType.Text;
                    detailCommand.CommandText = "INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)" +
                        "VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";

                    orderIdParam = CreateParameter(detailCommand, "@OrderID", order.OrderID, DbType.Int32);
                    var productID = CreateParameter(detailCommand, "@ProductID", detail.ProductID, DbType.Int32);
                    var unitPrice = CreateParameter(detailCommand, "@UnitPrice", detail.UnitPrice, DbType.Decimal);
                    var quantity = CreateParameter(detailCommand, "@Quantity", detail.Quantity, DbType.Double);
                    var discount = CreateParameter(detailCommand, "@Discount", detail.Discount, DbType.Double);

                    detailCommand.Parameters.AddRange(new[] { orderIdParam, productID, unitPrice, quantity, discount });

                    detailCommand.ExecuteNonQuery();
                }
            }
            else if (orderStatus == OrderStatus.InProgress || orderStatus == OrderStatus.Completed)
            {
                throw new InvalidOperationException(message: ExtensionMessage.CANNOT_UPDATE_INPROGRESS_OR_COMPLETED);
            }
            else
            {
                throw new InvalidOperationException(message: ExtensionMessage.UNKNOWN_STATUS);
            }
        }

        public void DeleteOrder(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var statusCommand = connection.CreateCommand();
            statusCommand.CommandType = CommandType.Text;
            statusCommand.CommandText = "SELECT OrderDate, ShippedDate FROM Orders WHERE OrderID = @OrderID";

            var orderIdParam = CreateParameter(statusCommand, "@OrderID", orderId, DbType.Int32);
            statusCommand.Parameters.Add(orderIdParam);

            DateTime? orderDate = default;
            DateTime? shippedDate = default;

            using (var reader = statusCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    orderDate = reader.IsDBNull(reader.GetOrdinal("OrderDate")) ? (DateTime?)null : (DateTime)reader["OrderDate"];
                    shippedDate = reader.IsDBNull(reader.GetOrdinal("ShippedDate")) ? (DateTime?)null : (DateTime)reader["ShippedDate"];
                }
                else
                {
                    throw new ArgumentException(message: ExtensionMessage.ORDER_NOT_FOUND);
                }
            }

            var orderStatus = GetOrderStatus(orderDate, shippedDate);

            if (orderStatus == OrderStatus.New || orderStatus == OrderStatus.InProgress)
            {
                var detailCommand = connection.CreateCommand();
                detailCommand.CommandType = CommandType.Text;
                detailCommand.CommandText = "DELETE FROM [Order Details] WHERE OrderID = @OrderID";

                orderIdParam = CreateParameter(detailCommand, "@OrderID", orderId, DbType.Int32);
                detailCommand.Parameters.Add(orderIdParam);

                detailCommand.ExecuteNonQuery();

                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandType = CommandType.Text;
                deleteCommand.CommandText = "DELETE FROM Orders WHERE OrderID = @OrderID";
                orderIdParam = CreateParameter(deleteCommand, "@OrderID", orderId, DbType.Int32);
                deleteCommand.Parameters.Add(orderIdParam);

                deleteCommand.ExecuteNonQuery();
            }
            else if (orderStatus == OrderStatus.Completed)
            {
                throw new InvalidOperationException(message: ExtensionMessage.CANNOT_DELETE_COMPLETED);
            }
            else
            {
                throw new InvalidOperationException(message: ExtensionMessage.ORDER_NOT_FOUND);
            }
        }

        public void MarkOrderAsInProgress(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE Orders SET OrderDate = @OrderDate WHERE OrderID = @OrderID";

            var orderDate = CreateParameter(command, "@OrderDate", DateTime.Now, DbType.DateTime);
            var orderID = CreateParameter(command, "@OrderID", orderId, DbType.Int32);

            command.Parameters.AddRange(new[] { orderDate, orderID });

            command.ExecuteNonQuery();
        }

        public void MarkOrderAsCompleted(int orderId)
        {
            using var connection = CreateAndOpenConnection();

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE Orders SET ShippedDate = @ShippedDate WHERE OrderID = @OrderID";

            var shippedDate = CreateParameter(command, "@ShippedDate", DateTime.Now, DbType.DateTime);
            var orderID = CreateParameter(command, "@OrderID", orderId, DbType.Int32);

            command.Parameters.AddRange(new[] { shippedDate, orderID });

            command.ExecuteNonQuery();
        }

        public OrderStatisticsView GetOrderStatistics(string customerID, int orderID)
        {
            using var connection = CreateAndOpenConnection();

            var cohCommand = connection.CreateCommand();
            cohCommand.CommandType = CommandType.StoredProcedure;
            cohCommand.CommandText = "CustOrderHist";

            var custIdParam = CreateParameter(cohCommand, "@CustomerID", customerID, DbType.String);
            cohCommand.Parameters.Add(custIdParam);

            OrderStatisticsView statistics = new();

            using (var custOrderHistReader = cohCommand.ExecuteReader())
            {

                while (custOrderHistReader.Read())
                {
                    var custOrderHist = new CustOrderHist
                    {
                        ProductName = custOrderHistReader["ProductName"].ToString(),
                        Total = Convert.ToInt32(custOrderHistReader["Total"])
                    };

                    statistics.CustOrderHistRes.Add(custOrderHist);
                }
            }

            var codCommand = connection.CreateCommand();
            codCommand.CommandType = CommandType.StoredProcedure;
            codCommand.CommandText = "CustOrdersDetail";

            var orderIdParam = CreateParameter(codCommand, "@OrderID", orderID, DbType.Int32);
            codCommand.Parameters.Add(orderIdParam);

            using var custOrdersDetailReader = codCommand.ExecuteReader();

            while (custOrdersDetailReader.Read())
            {
                var custOrdersDetail = new CustOrdersDetail
                {
                    ProductName = custOrdersDetailReader["ProductName"].ToString(),
                    Quantity = Convert.ToInt32(custOrdersDetailReader["Quantity"]),
                    UnitPrice = Convert.ToDecimal(custOrdersDetailReader["UnitPrice"]),
                    Discount = Convert.ToInt32(custOrdersDetailReader["Discount"]),
                    ExtendedPrice = Convert.ToDecimal(custOrdersDetailReader["ExtendedPrice"])
                };

                statistics.CustOrdersDetailRes.Add(custOrdersDetail);
            }

            return statistics;
        }
    }
}
