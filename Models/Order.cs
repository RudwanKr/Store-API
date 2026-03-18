using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Order
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public DateTime Date { get; set; }
        [NotMapped]
        public List<Product> products { get; set; }
        public Customer? customer { get; set; }
        public double TotalPrice { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public Order(int ID, DateTime Date, List<Product> products, Customer customer)
        {
            this.ID = ID;
            this.Date = Date;
            this.products = products;
            this.customer = customer;
        }

        public Order()
        {
            this.products = new List<Product>();
        }

        private double caclulateTotalPrice()
        {
            // Use UnitPrice from OrderDetails if available, otherwise Product.Price
            return OrderDetails?.Sum(od => (od.UnitPrice != 0 ? od.UnitPrice : od.Product.Price) * od.Quantity) ?? 0;
        }

        public void printOrderInvoice()
        {
            int IDWidth = 5;
            int nameWidth = 20;
            int priceWidth = 10;
            int qtyWidth = 10;

            Console.WriteLine("===== Order invoice =====");
            Console.WriteLine($"Order Number: {this.ID}");
            Console.WriteLine($"Customer Name: {this.customer.Name}");
            Console.WriteLine($"Order date: {this.Date.ToString()}");
            Console.WriteLine("Products:");
            Console.WriteLine(
                $"{"#".PadRight(IDWidth)}" +
                $"{"Name".PadRight(nameWidth)}" +
                $"{"Price".PadRight(priceWidth)}" +
                $"{"Quantity".PadRight(qtyWidth)}"
            );
            short i = 1;
            if (products.Count > 0)
            {
                foreach (Product p in products)
                {
                    Console.WriteLine(i.ToString().PadRight(IDWidth) + p.printOrderProduct());
                    i++;
                }
            }
            else Console.WriteLine("No products in this order.");
            Console.WriteLine($"\nTotal price : {this.TotalPrice}");
        }

        public string printOrder()
        {
            int IDWidth = 5;
            int nameWidth = 20;
            int DateWidth = 25;
            int totalPriceWidth = 10;

            return $"{this.ID.ToString().PadRight(IDWidth)}" +
                   $"{this.customer.Name.PadRight(nameWidth)}" +
                   $"{this.Date.ToString().PadRight(DateWidth)}" +
            $"{caclulateTotalPrice().ToString().PadRight(totalPriceWidth)}";
        }
    }

}
