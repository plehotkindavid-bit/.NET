using API.Data;
using API.Enetites;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class ProductService
{
    private readonly StoreContext _context;

    public ProductService(StoreContext context)
    {
        _context = context;
    }

    public Task<Product?> GetByIdAsync(int productId)
    {
        return _context.Products
            .FirstOrDefaultAsync(product => product.Id == productId);
    }

    public Task<Product?> GetByExactNameAsync(string name)
    {
        return _context.Products
            .FirstOrDefaultAsync(product => product.Name == name);
    }
}
