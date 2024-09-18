using CRMWeb.Models;

namespace CRMWeb.Services
{
    public class AnalyticsService
    {
        // Helper method to calculate how many samples are left for products
        public Dictionary<string, int> GetProductSampleDistribution(List<Appointment> appointments)
        {
            var productDistribution = new Dictionary<string, int>();

            foreach (var appointment in appointments)
            {
                if (appointment.AppointmentProducts == null) continue;

                foreach (var product in appointment.AppointmentProducts)
                {
                    // Check if Product.Name is null or empty
                    if (product.Product != null && !string.IsNullOrEmpty(product.Product.Name)) 
                    {
                        if (productDistribution.ContainsKey(product.Product.Name))
                        {
                            productDistribution[product.Product.Name] += product.QuantityLeft;
                        }
                        else
                        {
                            productDistribution[product.Product.Name] = product.QuantityLeft;
                        }
                    }
                }
            }

            return productDistribution;
        }

        // Helper method to calculate how many appointments are scheduled for doctors
        public Dictionary<string, int> GetDoctorVisitDistribution(List<Appointment> appointments)
        {
            var doctorVisitDistribution = appointments
                .Where(a => a.Doctor != null && !string.IsNullOrEmpty(a.Doctor.Name))
                .GroupBy(a => a.Doctor!.Name!)
                .ToDictionary(g => g.Key, g => g.Count());

            return doctorVisitDistribution;
        }
    }
}
