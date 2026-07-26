using Microsoft.AspNetCore.Mvc;
using API.Cores;
using API.Data;
using API.Enetites;
using API.Services;

namespace API.Data;


[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly StoreContext _context;
    private readonly ProductService _productService;

    public ProductsController(StoreContext context, ProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        return ToHttpResult(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindProduct(
        [FromQuery] int? productId,
        [FromQuery] string? name)
    {
        var result = await _productService.SearchAsync(productId, name);

        return ToHttpResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var result = await _productService.GetAllAsync();

        return ToHttpResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product
        );
    }

    private IActionResult ToHttpResult(CoreResult result)
    {
        return result.Status switch
        {
            CoreStatuses.Success => Ok(result),
            CoreStatuses.NotFound => NotFound(result),
            CoreStatuses.NeedsData => BadRequest(result),
            CoreStatuses.Rejected => BadRequest(result),
            CoreStatuses.Error => StatusCode(
                StatusCodes.Status500InternalServerError,
                result),
            _ => StatusCode(
                StatusCodes.Status500InternalServerError,
                CoreResultFactory.Error(
                    "CORE-PRODUCT-001",
                    "map_product_result",
                    "Unable to process product result."))
        };
    }
}
