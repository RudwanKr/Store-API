using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.DTOs.ProductDtos;
using Store_API.Models;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDBContext _context;
        public ProductsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
        {
            var products = await _context.Products.AsNoTracking()
                .Select(p => new ProductResponse
                {
                    ID = p.ID,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                })
                .ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product is null ? NotFound(new { message = $"Product with ID {id} does not exist." }) :
                Ok(new ProductResponse
                {
                    ID = product.ID,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Price = product.Price,

                });
        }

        [HttpPost("add")]
        public async Task<ActionResult<ProductResponse>> Add(ProductCreateRequest request)
        {
            var productEntity = new Product
            {
                Name = request.Name,
                Quantity = request.Quantity,
                Price = request.Price
            };
            _context.Products.Add(productEntity);
            await _context.SaveChangesAsync();

            var response = new ProductResponse
            {
                ID = productEntity.ID,
                Name = productEntity.Name,
                Quantity = productEntity.Quantity,
                Price = productEntity.Price
            };

            return CreatedAtAction(nameof(GetById), new { id = response.ID }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCreateRequest request)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound($"Product with ID {id} not found.");

            existingProduct.Name = request.Name;
            existingProduct.Quantity = request.Quantity;
            existingProduct.Price = request.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/update-stock")]
        public async Task<IActionResult> UpdateProductQuantity(int id, [FromQuery] int newQuantity)
        {
            if (newQuantity < 0) return BadRequest("Quantity cannot be negative.");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Quantity = newQuantity;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stock updated successfully", currentStock = product.Quantity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.OrderDetails)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (product == null) return NotFound();

            if (product.OrderDetails.Any())
            {
                return BadRequest("Cannot delete product because it is linked to existing orders.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("filterByPrice")]
        public async Task<ActionResult<List<ProductResponse>>> FilterByPrice([FromQuery] double minPrice, [FromQuery] double maxPrice)
        {
            var products = await _context.Products
                .AsNoTracking()
                .Select(p => new ProductResponse
                {
                    ID = p.ID,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                })
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();

            return Ok(products);
        }
        public async Task<ActionResult<ProductResponse>> GetMostProductQuantity()
        {
            var product = await _context.Products
                .AsNoTracking()
                .OrderByDescending(p => p.Quantity)
                .FirstOrDefaultAsync();

            return product is null ? NotFound() :
                Ok(new ProductResponse
                {
                    ID = product.ID,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Price = product.Price,
                });
        }
        
    }
}