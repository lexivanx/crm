namespace CRMWeb.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public DateTime Expiry { get; set; }

        public ICollection<AppointmentProduct>? AppointmentProducts { get; set; }
        public ICollection<UserProduct>? UserProducts { get; set; }
    }
}
