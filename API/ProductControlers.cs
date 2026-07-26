using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data ; 
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
    public async Task<ActionResult> GetProduct(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                status = "needs_data",
                missingData = new[] { "productId" }
            });
        }

        Product? product;

        try
        {
            product = await _productService.GetByIdAsync(id);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "error"
            });
        }

        if (product == null)
        {
            return NotFound(new
            {
                status = "not_found",
                productId = id
            });
        }

        return Ok(new
        {
            status = "success",
            data = product
        });
    }

    [HttpGet]
    public async Task<ActionResult> FindProduct([FromQuery] int? productId, [FromQuery] string? name)
    {
        if (productId.HasValue)
        {
            return await GetProduct(productId.Value);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new
            {
                status = "needs_data",
                missingData = new[] { "productId or exact name" }
            });
        }

        Product? product;

        try
        {
            product = await _productService.GetByExactNameAsync(name);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "error"
            });
        }

        if (product == null)
        {
            return NotFound(new
            {
                status = "not_found",
                name
            });
        }

        return Ok(new
        {
            status = "success",
            data = product
        });
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
}
