namespace CRMWeb.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required int StreetNum { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
