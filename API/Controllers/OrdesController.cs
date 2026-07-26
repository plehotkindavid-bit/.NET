using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder(
        CreateOrderRequest request
    )
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(
                request.ProductId,
                request.Quantity
            );

            if (order == null)
            {
                return NotFound(new
                {
                    status = "not_found",
                    productId = request.ProductId
                });
            }

            return Ok(new
            {
                status = "success",
                data = order
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                status = "error",
                message = exception.Message
            });
        }
    }
    [HttpGet]
public async Task<ActionResult> GetOrders()
{
    var orders = await _orderService.GetOrdersAsync();

    return Ok (new
    {
        status = "success",
        data = orders
    });
}
}

public class CreateOrderRequest
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

}