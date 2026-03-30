using Store_API.DTOs.ProductDtos;

namespace Store_API.Services.Abstractions
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken ct = default);
        Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProductResponse> AddAsync(ProductCreateRequest request, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, ProductCreateRequest request, CancellationToken ct = default);
        Task<bool> UpdateStockAsync(int id, int newQuantity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<ProductResponse>> FilterByPriceAsync(double minPrice, double maxPrice, CancellationToken ct = default);
        Task<ProductResponse> GetMostProductQuantityAsync(CancellationToken ct = default);
    }
}
