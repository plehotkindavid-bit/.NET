using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class OrderService
{
    private readonly StoreContext _context;

    public OrderService(StoreContext context)
    {
        _context = context;
    }

    public async Task<Order?> CreateOrderAsync(
        int productId,
        int quantity
    )
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == productId);

        if (product == null)
        {
            return null;
        }

        if (quantity <= 0)
        {
            throw new InvalidOperationException(
                "Количество должно быть больше нуля."
            );
        }

        if (product.QuantitiesInStock < quantity)
        {
            throw new InvalidOperationException(
                "Недостаточно товара на складе."
            );
        }

        var order = new Order
        {
            ProductId = product.Id,
            Quantity = quantity,
            UnitPrice = product.Price,
            TotalPrice = product.Price * quantity,
            CreatedAt = DateTime.UtcNow
        };

        product.QuantitiesInStock -= quantity;

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        return order;
    }
    public async Task<List<Order>> GetOrdersAsync()
{
    return await _context.Orders
        .OrderByDescending(order => order.CreatedAt)
        .ToListAsync();
}
}