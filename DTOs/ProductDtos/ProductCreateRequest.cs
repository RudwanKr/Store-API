namespace Store_API.DTOs.ProductDtos
{
    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
