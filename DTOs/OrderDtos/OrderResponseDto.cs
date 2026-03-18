namespace Store_API.DTOs.OrderDtos
{
    public class OrderResponseDto
    {
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public double TotalPrice { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
    }

    public class OrderItemResponseDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
