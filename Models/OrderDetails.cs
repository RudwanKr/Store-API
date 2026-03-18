using System.Text.Json.Serialization;

namespace Store_API.Models
{
    public class OrderDetails
    {
        public int ID { get; set; }

        public int ProductID { get; set; }
        public int OrderID { get; set; }
        public Product Product { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
