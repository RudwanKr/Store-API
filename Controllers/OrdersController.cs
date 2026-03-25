using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.DTOs.OrderDtos;
using Store_API.Models;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase 
    {
        private readonly AppDBContext _context;

        public OrdersController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> AddOrder(OrderRequestDto dto, CancellationToken ct = default)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerID);
            if (customer == null) return BadRequest("Customer not found.");

            var newOrder = new Order
            {
                CustomerID = dto.CustomerID,
                Date = DateTime.Now,
                OrderDetails = new List<OrderDetails>()
            };

            double calculatedTotal = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductID);
                if (product == null) return BadRequest($"Product {item.ProductID} not found.");

                if (product.Quantity < item.Quantity)
                    return BadRequest($"Not enough stock for {product.Name}.");

                product.Quantity -= item.Quantity;

                var detail = new OrderDetails
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                newOrder.OrderDetails.Add(detail);
                calculatedTotal += (detail.UnitPrice * detail.Quantity);
            }

            newOrder.TotalPrice = calculatedTotal;

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync(ct);

            var response = new OrderResponseDto
            {
                ID = newOrder.ID,
                CustomerName = customer.Name,
                Date = newOrder.Date,
                TotalPrice = newOrder.TotalPrice,
                Items = newOrder.OrderDetails.Select(od => new OrderItemResponseDto
                {
                    ProductName = _context.Products.Find(od.ProductID)?.Name ?? "Unknown Product",
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Select( o => new OrderResponseDto
                {
                    ID = o.ID,
                    CustomerName = o.customer.Name ?? "Unknown customer",
                    Date = o.Date,
                    TotalPrice = o.TotalPrice,
                    Items = o.OrderDetails.Select(od => new OrderItemResponseDto
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("filterByDate")]
        public async Task<ActionResult<List<OrderResponseDto>>> FilterByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var orders = await _context.Orders.AsNoTracking()
                .Where(o => o.Date >= startDate && o.Date <= endDate)
                .Select(o => new OrderResponseDto
                {
                    ID = o.ID,
                    CustomerName = o.customer.Name,
                    Date = o.Date,
                    TotalPrice = o.TotalPrice,
                    Items = o.OrderDetails.Select(od => new OrderItemResponseDto
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("filterByCustomerName")]
        public async Task<ActionResult<List<OrderResponseDto>>> FilterByCustomerName([FromQuery] string customerName)
        {
            var orders = await _context.Orders.AsNoTracking()
                .Where(o => o.customer.Name == customerName)
                .Select(o => new OrderResponseDto
                {
                    ID = o.ID,
                    CustomerName = o.customer.Name,
                    Date = o.Date,
                    TotalPrice = o.TotalPrice,
                    Items = o.OrderDetails.Select(od => new OrderItemResponseDto
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{orderID}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderByID(int orderID)
        {
            var order = await _context.Orders.AsNoTracking()
                .Where(o => o.ID == orderID)
                .Select(o => new OrderResponseDto
                {
                    ID = o.ID,
                    CustomerName = o.customer.Name,
                    Date = o.Date,
                    TotalPrice = o.TotalPrice,
                    Items = o.OrderDetails.Select(od => new OrderItemResponseDto
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return order is null? NotFound(new { message = $"Order with ID {orderID} does not exist." }) : Ok(order);
        }
    }
}