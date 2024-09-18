namespace CRMWeb.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }

        public int DoctorID { get; set; }
        public Doctor? Doctor { get; set; }

        public int UserID { get; set; }
        public User? User { get; set; }

        public ICollection<AppointmentProduct>? AppointmentProducts { get; set; }
    }
}
