using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.CustomerDtos;

namespace Store_API.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponse>> GetAll();
        Task<CustomerResponse> GetByIdAsync(int id);
        Task<CustomerCreateRequest> GetCustomerByName(string name);
        Task<CustomerCreateRequest> GetCustomerByPhone(string phone);
        Task<CustomerResponse> AddAsync(CustomerCreateRequest request, CancellationToken ct = default);
        Task<IActionResult> UpdateAsync(int id, CustomerCreateRequest request, CancellationToken ct = default);
        Task<IActionResult> DeleteAsync(int id, CancellationToken ct = default);
    }
}
