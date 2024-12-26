using FluentAssertions;
using Northwind.API;

namespace Northwind.Tests.OrderServiceTests
{
    public class GetOrdersTests : IClassFixture<TestServerFixture>
    {
        public readonly NorthwindService.NorthwindServiceClient _client;

        public GetOrdersTests(TestServerFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOrderList()
        {
            var request = new Google.Protobuf.WellKnownTypes.Empty();

            var result = await _client.GetOrdersAsync(request);

            result.Should().NotBeNull();
            result.Orders.Should().NotBeNullOrEmpty();
        }
    }
}