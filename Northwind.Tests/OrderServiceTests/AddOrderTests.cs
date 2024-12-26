using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Northwind.API;

namespace Northwind.Tests.OrderServiceTests
{
    public class AddOrderTests : IClassFixture<TestServerFixture>
    {
        public readonly NorthwindService.NorthwindServiceClient _client;

        public AddOrderTests(TestServerFixture fixture)
        {
            _client = fixture.Client;
        }

        [Theory]
        [MemberData(nameof(OrderDataToAdd))]
        public async Task AddOrder_ShouldReturnOrderId(AddOrderRequest request)
        {
            var result = await _client.AddOrderAsync(request);

            result.Should().NotBeNull();
            result.OrderId.Should().BePositive();
        }

        public static IEnumerable<object[]> OrderDataToAdd =>
           new List<object[]>
           {
                new object[]
                {
                    new AddOrderRequest
                    {
                        Order = new OrderCreate
                            {
                                CustomerID = "FISSA",
                                EmployeeID = 2,
                                OrderDate = null,
                                RequiredDate = Timestamp.FromDateTime(new DateTime(2024, 12, 31).ToUniversalTime()),
                                ShippedDate = null,
                                ShipVia = 1,
                                Freight = (double) 55.55m,
                                ShipName = "TestShipName55",
                                ShipAddress = "TestShipAddress55",
                                ShipCity = "TestShipCity55",
                                ShipCountry = "USA",
                                ShipRegion = "RU",
                                ShipPostalCode = "555555"
                            }
                    }.AddOrderDetails(new List<OrderDetailToCreateOrder>
                        {
                           new() { ProductID = 5, UnitPrice = 5, Quantity = 5, Discount = 0.05 },
                           new() { ProductID = 1, UnitPrice = 1, Quantity = 1, Discount = 0.01 }
                        })

                }
           };
    }

    public static class AddOrderRequestExtensions
    {
        public static AddOrderRequest AddOrderDetails(this AddOrderRequest request, IEnumerable<OrderDetailToCreateOrder> orderDetails)
        {
            request.OrderDetails.AddRange(orderDetails);

            return request;
        }
    }
}