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
        public async Task<ActionResult<OrderResponseDto>> AddOrder(OrderRequestDto request)
        {
            var newOrder = new Order
            {
                CustomerID = request.CustomerID,
                Date = DateTime.Now,
                OrderDetails = new List<OrderDetails>()
            };

            double calculatedTotal = 0;

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductID);

                if (product == null) return BadRequest($"Product {item.ProductID} not found.");
                if (product.Quantity < item.Quantity) return BadRequest($"Not enough stock for {product.Name}.");

                product.decreaseQuantity(item.Quantity);

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
            await _context.SaveChangesAsync();

            return Ok(new OrderResponseDto
            {
                ID = newOrder.ID,
                Date = newOrder.Date,
                TotalPrice = newOrder.TotalPrice,
                CustomerName = newOrder.customer.Name,
                Items = newOrder.OrderDetails.Select(od => new OrderItemResponseDto
                {
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Select( o => new OrderResponseDto
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