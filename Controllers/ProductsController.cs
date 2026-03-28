using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.DTOs.ProductDtos;
using Store_API.Models;
using Store_API.Services.Abstractions;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IProductService _productService;
        public ProductsController(AppDBContext context,IProductService productService)
        {
            _context = context;
            _productService = productService;
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
        public async Task<ActionResult<ProductResponse>> Add(ProductCreateRequest request, CancellationToken ct = default)
        {
            var productEntity = new Product
            {
                Name = request.Name,
                Quantity = request.Quantity,
                Price = request.Price
            };
            _context.Products.Add(productEntity);
            await _context.SaveChangesAsync(ct);

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
        public async Task<IActionResult> Update(int id, ProductCreateRequest request, CancellationToken ct = default)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound($"Product with ID {id} not found.");

            existingProduct.Name = request.Name;
            existingProduct.Quantity = request.Quantity;
            existingProduct.Price = request.Price;

            await _context.SaveChangesAsync(ct);

            return NoContent();
        }

        [HttpPatch("{id:int}/update-stock")]
        public async Task<IActionResult> UpdateProductQuantity(int id, [FromQuery] int newQuantity, CancellationToken ct)
        {
            var success = await _productService.UpdateStockAsync(id, newQuantity, ct);

            if (!success)
            {
                return BadRequest(new { message = "Update failed. Check if ID exists or quantity is valid." });
            }

            return Ok(new { message = "Stock updated successfully", currentStock = newQuantity });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
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
            await _context.SaveChangesAsync(ct);

            return NoContent();
        }

        [HttpGet("filterByPrice")]
        public async Task<ActionResult<List<ProductResponse>>> FilterByPrice([FromQuery] double minPrice, [FromQuery] double maxPrice,CancellationToken ct = default)
        {
            var results = await _productService.FilterByPriceAsync(minPrice, maxPrice, ct);
            return Ok(results);
        }
        [HttpGet("most-quantity")]
        public async Task<ActionResult<ProductResponse>> GetMostQuantity(CancellationToken ct)
        {
            var result = await _productService.GetMostProductQuantityAsync(ct);

            if (result == null)
            {
                return NotFound(new { message = "No products found in the database." });
            }

            return Ok(result);
        }

    }
}