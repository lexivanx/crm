namespace CRMWeb.Models
{
    public class UserProduct
    {
        public int UserID { get; set; }
        public User? User { get; set; }

        public int ProductID { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}
