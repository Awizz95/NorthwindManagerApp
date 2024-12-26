using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Northwind.API;

namespace Northwind.Tests.OrderServiceTests
{
    public class DeleteOrderTests : IClassFixture<TestServerFixture>
    {
        public readonly NorthwindService.NorthwindServiceClient _client;

        public DeleteOrderTests(TestServerFixture fixture)
        {
            _client = fixture.Client;
        }

        [Theory]
        [MemberData(nameof(OrderIdToDelete))]
        public async Task DeleteOrder_WithOrderIdParameter_ShouldDeleteOrder(DeleteOrderRequest request)
        {
            var result = await _client.DeleteOrderAsync(request);

            result.Should().NotBeNull()
                .And.BeOfType<Empty>();
        }

        public static IEnumerable<object[]> OrderIdToDelete =>
          new List<object[]>
          {
                new object[]
                {
                    new DeleteOrderRequest
                    {
                        OrderId = 14094
                    }
                }
          };
    }
}
