using Microsoft.AspNetCore.Mvc;
using Northwind.API.Requests;
using NorthwindBL.Interfaces;
using NorthwindDAL.Models;
using NorthwindDAL.Views;

namespace Northwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IAppLogic _appLogic;

        public OrderController(IAppLogic appLogic, ILogger<OrderController> logger)
        {
            _logger = logger;
            _appLogic = appLogic;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>All orders</returns>
        [HttpGet("getorders", Name = "GetOrders")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetOrders()
        {
            IEnumerable<OrderWithStatusView> orders = _appLogic.GetOrders();

            return Ok(orders);
        }

        /// <summary>
        /// Get searched order with order details
        /// </summary>
        /// <param name="id">Searching order id</param>
        /// <returns>Order detailed with order details</returns>
        [HttpPost("getorder{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetOrderDetailed(int id)
        {
            GetAllOrderDetailsView? order = _appLogic.GetAllOrderDetails(id);

            if (order is null)
            {
                return BadRequest();
            }

            return Ok(order);
        }

        /// <summary>
        /// Add order to DB
        /// </summary>
        /// <param name="request">The order request</param>
        /// <returns>Order id</returns>
        [HttpPost("addorder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddOrder([FromBody] CreateOrderRequest request)
        {
            if (request is null || request.Order is null)
            {
                return BadRequest();
            }

            int orderID;

            try
            {
                orderID = _appLogic.CreateOrder(request.Order, request.OrderDetails);
            }
            catch
            {
                return BadRequest();
            }

            return Ok(orderID);
        }

        /// <summary>
        /// Delete order from DB
        /// </summary>
        /// <param name="id">Order id to delete</param>
        /// <returns></returns>
        [HttpPost("deleteorder{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteOrder(int id)
        {
            try
            {
                _appLogic.DeleteOrder(id);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Edit searched order
        /// </summary>
        /// <param name="newOrder">Order model</param>
        /// <returns></returns>
        [HttpPost("editorder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult EditOrder(Order newOrder)
        {
            try
            {
                _appLogic.UpdateOrder(newOrder);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
