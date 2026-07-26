using API.Cores;
using API.Data;
using API.Dtos;
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
        string operationId,
        int? productId,
        string? exactName)
    {
        if (productId.HasValue)
        {
            return await GetByIdAsync(operationId, productId.Value);
        }

        if (string.IsNullOrWhiteSpace(exactName))
        {
            return CoreResultFactory.NeedsData(
                Module,
                operationId,
                GetByNameOperation,
                ["productId or name"]);
        }

        return await GetByExactNameAsync(operationId, exactName);
    }

    public async Task<CoreResult> GetByIdAsync(
        string operationId,
        int productId)
    {
        if (productId <= 0)
        {
            return CoreResultFactory.Rejected(
                Module,
                operationId,
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
                    operationId,
                    GetByIdOperation,
                    "Product was not found by ID.",
                    databaseChecked: true,
                    databaseOperation);
            }

            return CoreResultFactory.Success(
                Module,
                operationId,
                GetByIdOperation,
                new { product = ToReadDto(product) },
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
                operationId,
                GetByIdOperation,
                "Unable to read product data.",
                databaseChecked: false,
                databaseOperation:
                    "EF Core product lookup by ID failed.");
        }
    }

    public async Task<CoreResult> GetByExactNameAsync(
        string operationId,
        string exactName)
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
                    operationId,
                    GetByNameOperation,
                    "Product was not found by exact name.",
                    databaseChecked: true,
                    databaseOperation);
            }

            return CoreResultFactory.Success(
                Module,
                operationId,
                GetByNameOperation,
                new { product = ToReadDto(product) },
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
                operationId,
                GetByNameOperation,
                "Unable to read product data.",
                databaseChecked: false,
                databaseOperation:
                    "EF Core product lookup by exact name failed.");
        }
    }

    public async Task<CoreResult> GetAllAsync(string operationId)
    {
        const string databaseOperation =
            "Queried StoreContext.Products with ToListAsync.";

        try
        {
            var products = await _context.Products.ToListAsync();
            var productDtos = products
                .Select(ToReadDto)
                .ToList();

            return CoreResultFactory.Success(
                Module,
                operationId,
                GetAllOperation,
                new { products = productDtos },
                databaseChecked: true,
                databaseOperation: databaseOperation,
                recordIds: products.Select(product => (object)product.Id));
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Product list query failed.");

            return CoreResultFactory.Error(
                Module,
                operationId,
                GetAllOperation,
                "Unable to read product data.",
                databaseChecked: false,
                databaseOperation:
                    "EF Core product list query failed.");
        }
    }

    private static ProductReadDto ToReadDto(Product product)
    {
        return new ProductReadDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            PictureUrl = product.PictureUrl,
            Type = product.Type,
            Brand = product.Brand
        };
    }
}
