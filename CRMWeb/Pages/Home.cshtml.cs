using CRMWeb.Data;
using CRMWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRMWeb.Pages
{
    [Authorize]
    public class HomeModel : PageModel
    {
        private readonly CRMContext _context;

        public HomeModel(CRMContext context)
        {
            _context = context;
            Appointments = new List<Appointment>();
            AppointmentProducts = new List<AppointmentProduct>();
        }

        public List<Appointment> Appointments { get; set; }
        public List<AppointmentProduct> AppointmentProducts { get; set; }

        public void OnGet()
        {
            // Get logged-in user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User ID claim is missing or invalid.");
            }

            // Fetch appointments for logged-in user
            Appointments = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.AppointmentProducts!)
                .ThenInclude(ap => ap.Product!)
                .Where(a => a.UserID == userId)
                .ToList();

            // Get related products for appointments
            AppointmentProducts = _context.AppointmentProducts
                .Include(ap => ap.Product)
                .Where(ap => Appointments.Select(a => a.Id).Contains(ap.AppointmentID))
                .ToList();
        }

        public void OnPost(string filter)
        {
            // Get logged-in user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User ID claim is missing or invalid.");
            }

            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.AppointmentProducts!)
                .ThenInclude(ap => ap.Product!)
                .Where(a => a.UserID == userId);

            if (filter == "Upcoming")
            {
                Appointments = query.Where(a => a.AppointmentDate > DateTime.Today).ToList();
            }
            else if (filter == "Past")
            {
                Appointments = query.Where(a => a.AppointmentDate < DateTime.Today).ToList();
            }
            else
            {
                Appointments = query.ToList();
            }

            // Get related products for appointments
            AppointmentProducts = _context.AppointmentProducts
                .Include(ap => ap.Product)
                .Where(ap => Appointments.Select(a => a.Id).Contains(ap.AppointmentID))
                .ToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            // Find appointment by ID
            var appointment = _context.Appointments
                .Include(a => a.AppointmentProducts)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.AppointmentProducts != null)
            {
                // Remove AppointmentProducts and restore UserProducts
                foreach (var appointmentProduct in appointment.AppointmentProducts)
                {
                    var userProduct = _context.UserProducts.FirstOrDefault(up => up.ProductID == appointmentProduct.ProductID && up.UserID == appointment.UserID);

                    if (userProduct != null)
                    {
                        userProduct.Quantity += appointmentProduct.QuantityLeft;
                        _context.UserProducts.Update(userProduct);
                    }

                    _context.AppointmentProducts.Remove(appointmentProduct);
                }
            }

            // Remove appointment
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToPage();
        }
    }
}
