using System.Text.Json.Serialization;

namespace Store_API.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public List<OrderDetails> OrderDetails { get; set; } = new();
        public Product(int ID, string name, double price, int quantity)
        {
            this.ID = ID;
            this.Name = name;
            this.Price = price;
            this.Quantity = quantity;
        }

        public Product() { }

        public bool decreaseQuantity(int quantity)
        {
            if (this.Quantity < quantity)
            {
                return false;
            }
            this.Quantity -= quantity;
            return true;
        }
        public bool increaseQuantity(int quantity)
        {
            this.Quantity += quantity;
            return true;
        }

        public string printProduct(int idWidth = 5, int nameWidth = 20, int priceWidth = 10, int qtyWidth = 10)
        {
            return $"{ID.ToString().PadRight(idWidth)}" +
                   $"{Name.PadRight(nameWidth)}" +
                   $"{Price.ToString("F2").PadRight(priceWidth)}" +
                   $"{Quantity.ToString().PadRight(qtyWidth)}";
        }

        public string printOrderProduct(int nameWidth = 20, int priceWidth = 10, int qtyWidth = 10)
        {
            return $"{Name.PadRight(nameWidth)}" +
                   $"{Price.ToString("F2").PadRight(priceWidth)}" +
                   $"{Quantity.ToString().PadRight(qtyWidth)}";
        }
    }
}
