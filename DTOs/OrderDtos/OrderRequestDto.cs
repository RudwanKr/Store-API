namespace Store_API.DTOs.OrderDtos
{
    public class OrderRequestDto
    {
        public int CustomerID { get; set; }
        public List<OrderItemRequestDto> Items { get; set; }
    }
    public class OrderItemRequestDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
