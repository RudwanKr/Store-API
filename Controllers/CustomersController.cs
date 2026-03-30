using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.DTOs.CustomerDtos;
using Store_API.Models;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CustomersController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> Add(CustomerCreateRequest request, CancellationToken ct = default)
        {
            var customerEntity = new Customer
            {
                Name = request.Name,
                Phone = request.Phone
            };

            _context.Customers.Add(customerEntity);
            await _context.SaveChangesAsync(ct);

            var response = new CustomerResponse
            {
                ID = customerEntity.ID,
                Name = customerEntity.Name,
                Phone = customerEntity.Phone
            };

            return CreatedAtAction(nameof(GetById), new { id = response.ID }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll()
        {
            var customersDto = await _context.Customers.AsNoTracking()
                .Select(c => new CustomerResponse
                {
                    ID = c.ID,
                    Name = c.Name,
                    Phone = c.Phone
                })
                .ToListAsync();
            return Ok(customersDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound(new { message = $"Customer with ID {id} does not exist." });

            return Ok(new CustomerResponse
            {
                ID = customer.ID,
                Name = customer.Name,
                Phone = customer.Phone
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CustomerCreateRequest request, CancellationToken ct = default)
        {
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null) return NotFound($"Customer with ID {id} not found.");

            existingCustomer.Name = request.Name;
            existingCustomer.Phone = request.Phone;

            await _context.SaveChangesAsync(ct);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id,CancellationToken ct = default)
        {
            var customer = await _context.Customers
                .Include(p => p.Orders)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (customer == null) return NotFound();

            if (customer.Orders.Any())
            {
                return BadRequest("Cannot delete customer because it is linked to existing orders.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(ct);

            return NoContent();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<CustomerCreateRequest>> GetCustomerByName(string name)
        {
            var customer = await _context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            return customer is null ? NotFound(new { message = $"Customer with name {name} does not exist." }) :
                Ok(new CustomerResponse
                    {
                        ID = customer.ID,
                        Name = customer.Name,
                        Phone = customer.Phone
                    });
        }

        [HttpGet("phone/{phone}")]
        public async Task<ActionResult<CustomerCreateRequest>> GetCustomerByPhone(string phone)
        {
            var customer = await _context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == phone);

            return customer is null ? NotFound(new { message = $"Customer with phone {phone} does not exist." }) :
                Ok(new CustomerResponse
                {
                    ID = customer.ID,
                    Name = customer.Name,
                    Phone = customer.Phone
                });
        }
    }
}