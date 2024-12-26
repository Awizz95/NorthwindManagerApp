using FluentAssertions;
using Northwind.API;

namespace Northwind.Tests.OrderServiceTests
{
    public class GetOrderDetailedTests : IClassFixture<TestServerFixture>
    {
        public readonly NorthwindService.NorthwindServiceClient _client;
        public GetOrderDetailedTests(TestServerFixture fixture)
        {
            _client = fixture.Client;
        }

        [Theory]
        [MemberData(nameof(OrderIdToGetOrder))]
        public async Task GetOrderDetailed_WithOrderIdParameter_ShouldReturnOrder(GetOrderRequest request)
        {
            var result = await _client.GetOrderDetailedAsync(request);

            result.Should().NotBeNull();

            result.Should().Match<OrderDetailed>(x =>
            x.OrderId == 10718 &&
            x.CustomerID == "KOENE" &&
            x.EmployeeID == 1);

            result.OrderDetails.Should().NotBeNullOrEmpty()
                .And.HaveCount(x => x == 4)
                .And.Contain(x => x.ProductID == 16);
        }

        public static IEnumerable<object[]> OrderIdToGetOrder =>
          new List<object[]>
          {
                new object[]
                {
                    new GetOrderRequest
                    {
                        OrderId = 10718
                    }
                }
          };
    }
}
