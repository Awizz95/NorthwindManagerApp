using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Northwind.API;

namespace Northwind.Tests.OrderServiceTests
{
    public class EditOrderTests : IClassFixture<TestServerFixture>
    {
        public readonly NorthwindService.NorthwindServiceClient _client;

        public EditOrderTests(TestServerFixture fixture)
        {
            _client = fixture.Client;
        }

        [Theory]
        [MemberData(nameof(OrderDataToEdit))]
        public async Task EditOrder_WithNewOrderParameter_ShouldEditOrder(EditOrderRequest request)
        {
            var result = await _client.EditOrderAsync(request);

            result.Should().NotBeNull()
                .And.BeOfType<Empty>();
        }

        public static IEnumerable<object[]> OrderDataToEdit =>
           new List<object[]>
           {
                new object[]
                {
                    new EditOrderRequest
                    {
                        Order = new gOrder
                            {
                               OrderId = 14093,
                               CustomerID = "VINET",
                               EmployeeID = 3,
                               OrderDate = null,
                               RequiredDate = Timestamp.FromDateTime(new DateTime(2024, 12, 31).ToUniversalTime()),
                               ShippedDate = null,
                               ShipVia = 1,
                               Freight = 60.55,
                               ShipName = "TestShipName55",
                               ShipAddress = "TestShipAddress55",
                               ShipCity = "TestShipCity55",
                               ShipCountry = "USA",
                               ShipRegion = "RU",
                               ShipPostalCode = "555555"
                            }.AddOrderDetails(new List<OrderDetailToCreateOrder>
                                {
                                   new() { ProductID = 5, UnitPrice = 5, Quantity = 5, Discount = 0.05 },
                                   new() { ProductID = 1, UnitPrice = 1, Quantity = 1, Discount = 0.01 }
                            })
                    }
                }
           };
    }

    public static class gOrderExtensions
    {
        public static gOrder AddOrderDetails(this gOrder request, IEnumerable<OrderDetailToCreateOrder> orderDetails)
        {
            request.OrderDetails.AddRange(orderDetails);

            return request;
        }
    }
}