using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.ProductDtos;
using Store_API.MiddleWares;
using Store_API.Services.Abstractions;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll(CancellationToken ct)
        {
            var products = await _productService.GetAllAsync(ct);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetById(int id, CancellationToken ct)
        {
            var product = await _productService.GetByIdAsync(id, ct);

            return product is null
                ? NotFound(new { message = $"Product with ID {id} does not exist." })
                : Ok(product);
        }

        [HttpPost("add")]
        public async Task<ActionResult<ProductResponse>> Add(ProductCreateRequest request, CancellationToken ct = default)
        {
            var response = await _productService.AddAsync(request, ct);

            return CreatedAtAction(nameof(GetById), new { id = response.ID }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCreateRequest request, CancellationToken ct)
        {
            var success = await _productService.UpdateAsync(id, request, ct);
            if (!success) return NotFound(new { message = "Update failed. Product not found." });

            return Ok(new { message = "Product updated successfully" });
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
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _productService.DeleteAsync(id, ct);

            return success ? NoContent() : NotFound("The product was not found or is linked to orders.");
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

        [HttpGet("test-crash")]
        public IActionResult TestCrash()
        {
            throw new UnauthorizedAccessException("You aren't supposed to be here!");
        }
        [ApiKey]  // ONLY runs the check for this specific action!
        [HttpPost("test-apikey")]
        public IActionResult CreateSecretProduct() => Ok("Secret Product Created");

    }
}