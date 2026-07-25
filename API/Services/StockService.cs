using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class StockService
{
    private readonly StoreContext _context;

    public StockService(StoreContext context)
    {
        _context = context;
    }

    public async Task<int ?> ChangeStockAsync(
        int productId,
        int change
    )
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == productId);

        if (product == null)
        {
            return null;
        }

        int newQuantity = product.QuantitiesInStock + change;

        if (newQuantity < 0)
        {
            throw new InvalidOperationException(
                "Количество товара не может быть меньше нуля."
            );
        }

        product.QuantitiesInStock = newQuantity;

        await _context.SaveChangesAsync();

        return product.QuantitiesInStock;
    }
}