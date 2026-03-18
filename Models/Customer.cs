namespace Store_API.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<Order> Orders { get; set; }
        public Customer(int ID, string Name, string Phone)
        {
            this.ID = ID;
            this.Name = Name;
            this.Phone = Phone;
        }
        public Customer() { }
        public string printCustomer(int idWidth = 5, int nameWidth = 20, int phoneWidth = 15)
        {
            return $"{ID.ToString().PadRight(idWidth)}" +
                   $"{Name.PadRight(nameWidth)}" +
                   $"{Phone.PadRight(phoneWidth)}";
        }
    }

}
