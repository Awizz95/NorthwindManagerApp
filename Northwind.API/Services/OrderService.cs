using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NorthwindBL.Interfaces;
using NorthwindDAL.Models;
using NorthwindDAL.Views;

namespace Northwind.API.Services
{
    public class OrderService : NorthwindService.NorthwindServiceBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IAppLogic _appLogic;

        public OrderService(ILogger<OrderService> logger, IAppLogic appLogic, IMapper mapper)
        {
            _logger = logger;
            _appLogic = appLogic;
            _mapper = mapper;
        }

        public override Task<OrderList> GetOrders(Empty request, ServerCallContext context)
        {
            IEnumerable<OrderWithStatusView> orders = _appLogic.GetOrders();

            var orderList = new OrderList
            {
                Orders = { _mapper.Map<IEnumerable<OrderWithStatus>>(orders) }
            };

            return Task.FromResult(orderList);
        }

        public override Task<OrderIdResponse> AddOrder(AddOrderRequest request, ServerCallContext context)
        {
            int orderID = _appLogic.CreateOrder(_mapper.Map<OrderCreateView>(request.Order), _mapper.Map<IEnumerable<OrderDetailToCreateOrderView>>(request.OrderDetails));

            return Task.FromResult(new OrderIdResponse { OrderId = orderID});
        }

        public override Task<Empty> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
        {
            _appLogic.DeleteOrder(request.OrderId);

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> EditOrder(EditOrderRequest request, ServerCallContext context)
        {
            _appLogic.UpdateOrder(_mapper.Map<Order>(request.Order));

            return Task.FromResult(new Empty());
        }

        public override Task<OrderDetailed> GetOrderDetailed(GetOrderRequest request, ServerCallContext context)
        {
            GetAllOrderDetailsView? order = _appLogic.GetAllOrderDetails(request.OrderId);

            if (order is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Order was not found."));
            }

            return Task.FromResult(_mapper.Map<OrderDetailed>(order));
        }
    }
}
