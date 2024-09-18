namespace CRMWeb.Models
{
    public class AppointmentProduct
    {
        public int AppointmentID { get; set; }
        public Appointment? Appointment { get; set; }

        public int ProductID { get; set; }
        public Product? Product { get; set; }

        public int QuantityLeft { get; set; }
    }
}
