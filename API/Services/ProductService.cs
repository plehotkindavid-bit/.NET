using API.Cores;
using API.Data;
using API.Enetites;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class ProductService
{
    private const string Module = "CORE-PRODUCT-001";
    private const string GetByIdOperation = "get_product_by_id";
    private const string GetByNameOperation = "get_product_by_exact_name";
    private const string GetAllOperation = "get_products";

    private readonly StoreContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        StoreContext context,
        ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CoreResult> SearchAsync(
        int? productId,
        string? exactName)
    {
        if (productId.HasValue)
        {
            return await GetByIdAsync(productId.Value);
        }

        if (string.IsNullOrWhiteSpace(exactName))
        {
            return CoreResultFactory.NeedsData(
                Module,
                GetByNameOperation,
                ["productId or name"]);
        }

        return await GetByExactNameAsync(exactName);
    }

    public async Task<CoreResult> GetByIdAsync(int productId)
    {
        if (productId <= 0)
        {
            return CoreResultFactory.Rejected(
                Module,
                GetByIdOperation,
                "Product ID must be greater than zero.");
        }

        const string databaseOperation =
            "Queried StoreContext.Products with FirstOrDefaultAsync by product ID.";

        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == productId);

            if (product == null)
            {
                return CoreResultFactory.NotFound(
                    Module,
                    GetByIdOperation,
                    "Product was not found by ID.",
                    databaseChecked: true,
                    databaseOperation);
            }

            return CoreResultFactory.Success(
                Module,
                GetByIdOperation,
                product,
                databaseChecked: true,
                databaseOperation: databaseOperation,
                recordIds: [product.Id]);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Product lookup by ID failed.");

            return CoreResultFactory.Error(
                Module,
                GetByIdOperation,
                "Unable to read product data.",
                databaseChecked: false,
                databaseOperation:
                    "EF Core product lookup by ID failed.");
        }
    }

    public async Task<CoreResult> GetByExactNameAsync(string exactName)
    {
        const string databaseOperation =
            "Queried StoreContext.Products with FirstOrDefaultAsync by exact product name.";

        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Name == exactName);

            if (product == null)
            {
                return CoreResultFactory.NotFound(
                    Module,
                    GetByNameOperation,
                    "Product was not found by exact name.",
                    databaseChecked: true,
                    databaseOperation);
            }

            return CoreResultFactory.Success(
                Module,
                GetByNameOperation,
                product,
                databaseChecked: true,
                databaseOperation: databaseOperation,
                recordIds: [product.Id]);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Product lookup by exact name failed.");

            return CoreResultFactory.Error(
                Module,
                GetByNameOperation,
                "Unable to read product data.",
                databaseChecked: false,
                databaseOperation:
                    "EF Core product lookup by exact name failed.");
        }
    }

    public async Task<CoreResult> GetAllAsync()
    {
        const string databaseOperation =
            "Queried StoreContext.Products with ToListAsync.";

        try
        {
            var products = await _context.Products.ToListAsync();

            return CoreResultFactory.Success(
                Module,
                GetAllOperation,
                products,
                databaseChecked: true,
                databaseOperation: databaseOperation,
                recordIds: products.Select(product => product.Id));
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Product list query failed.");

            return CoreResultFactory.Error(
                Module,
                GetAllOperation,
                "Unable to read product data.",
                databaseChecked: false,
                databaseOperation:
                    "EF Core product list query failed.");
        }
    }
}
