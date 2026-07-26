using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data ; 
using API.Enetites; 
namespace API.Data;


[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly StoreContext _context;

    public ProductsController(StoreContext context)
    {
        _context = context;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetProduct(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == id);

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
[HttpGet]
public async Task<ActionResult<List<Product>>> GetProducts()
{
    var products = await _context.Products.ToListAsync();

    return Ok(products);
}
}