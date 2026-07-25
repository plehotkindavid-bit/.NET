using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class PriceService
{
    private readonly StoreContext _context;

    public PriceService(StoreContext context)
    {
        _context = context;
    }

    public async Task<int?> ChangePriceAsync(
        int productId,
        int newPrice
    )
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == productId);

        if (product == null)
        {
            return null;
        }

        if (newPrice <= 0)
        {
            throw new InvalidOperationException(
                "Цена должна быть больше нуля."
            );
        }

        product.Price = newPrice;

        await _context.SaveChangesAsync();

        return product.Price;
    }
}