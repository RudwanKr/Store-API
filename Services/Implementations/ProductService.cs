using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.DTOs.ProductDtos;
using Store_API.Models;
using Store_API.Services.Abstractions;

namespace Store_API.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDBContext _context;

        public ProductService(AppDBContext context) => _context = context;

        public async Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Products.AsNoTracking()
                .Select(p => new ProductResponse
                {
                    ID = p.ID,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                }).ToListAsync(ct);
        }
        public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _context.Products.FindAsync(id);
            return product is null ? null :
                new ProductResponse
                {
                    ID = product.ID,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Price = product.Price,
                };
        }
        public async Task<ProductResponse> AddAsync(ProductCreateRequest request, CancellationToken ct = default)
        {
            var product = new Product
            {
                Name = request.Name,
                Quantity = request.Quantity,
                Price = request.Price
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync(ct);

            return new ProductResponse
            {
                ID = product.ID,
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price
            };
        }
        public async Task<bool> UpdateAsync(int id, ProductCreateRequest request, CancellationToken ct = default)
        {
            int rowsAffected = await _context.Products
                .Where(p => p.ID == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Name, request.Name)
                    .SetProperty(p => p.Quantity, request.Quantity)
                    .SetProperty(p => p.Price, request.Price),
                ct);

            return rowsAffected > 0;
        }
        public async Task<bool> UpdateStockAsync(int id, int newQuantity, CancellationToken ct = default)
        {
            if (newQuantity < 0) return false;

            var product = await _context.Products.FindAsync(new object[] { id }, ct);
            if (product == null) return false;

            product.Quantity = newQuantity;
            await _context.SaveChangesAsync(ct);

            return true;
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var product = await _context.Products
                .Include(p => p.OrderDetails)
                .FirstOrDefaultAsync(p => p.ID == id, ct);

            if (product == null || product.OrderDetails.Any()) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(ct);
            return true;
        }
        public async Task<IEnumerable<ProductResponse>> FilterByPriceAsync(double minPrice, double maxPrice, CancellationToken ct = default)
        {
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .Select(p => new ProductResponse
                {
                    ID = p.ID,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                })
                .ToListAsync(ct);

            return products;
        }

        public async Task<ProductResponse> GetMostProductQuantityAsync(CancellationToken ct = default)
        {
            var product = await _context.Products
                .AsNoTracking()
                .OrderByDescending(p => p.Quantity)
                .Select(p => new ProductResponse
                {
                    ID = p.ID,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                })
                .FirstOrDefaultAsync(ct);

            return product;
        }
    }
}
