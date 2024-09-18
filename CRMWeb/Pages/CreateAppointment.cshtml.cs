using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CRMWeb.Data;
using CRMWeb.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CRMWeb.Pages
{
    [Authorize]
    public class CreateAppointmentModel : PageModel
    {
        private readonly CRMContext _context;

        public CreateAppointmentModel(CRMContext context)
        {
            _context = context;
            Doctors = new List<Doctor>();
            UserProducts = new List<UserProduct>();
        }

        [BindProperty]
        public int DoctorID { get; set; }

        [BindProperty]
        public List<int> SelectedProductIDs { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedQuantities { get; set; } = new List<int>();

        [BindProperty]
        public DateTime AppointmentDate { get; set; } = DateTime.Now;

        [BindProperty]
        public TimeSpan AppointmentTime { get; set; } = TimeSpan.Zero;

        public List<Doctor> Doctors { get; set; }
        public List<UserProduct> UserProducts { get; set; }

        public void OnGet()
        {
            // Fetch all doctors
            Doctors = _context.Doctors.ToList();

            // Fetch logged-in user's assigned products
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                UserProducts = _context.UserProducts
                    .Include(up => up.Product)
                    .Where(up => up.UserID == userId)
                    .ToList();
            }
        }

        public IActionResult OnPost()
        {
            // Refresh Doctors and UserProducts
            Doctors = _context.Doctors.ToList();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User ID claim is missing or invalid.");
            }

            UserProducts = _context.UserProducts
                .Include(up => up.Product)
                .Where(up => up.UserID == userId)
                .ToList();

            // Check if product and quantity selected
            if (SelectedProductIDs.Count == 0 || SelectedQuantities.Count == 0 || SelectedProductIDs.Count != SelectedQuantities.Count)
            {
                ModelState.AddModelError("", "Please select at least one product and provide a valid quantity.");
                return Page();
            }

            // Validate selected product and quantity
            for (int i = 0; i < SelectedProductIDs.Count; i++)
            {
                var productId = SelectedProductIDs[i];
                var quantity = SelectedQuantities[i];

                var userProduct = UserProducts.FirstOrDefault(up => up.ProductID == productId);
                if (userProduct == null || quantity <= 0 || userProduct.Quantity < quantity)
                {
                    ModelState.AddModelError("", $"Not enough quantity available for product {userProduct?.Product?.Name ?? "Unknown"}.");
                    return Page();
                }
            }

            // Create new appointment
            var appointment = new Appointment
            {
                DoctorID = DoctorID,
                UserID = userId,
                AppointmentDate = AppointmentDate,
                AppointmentTime = AppointmentTime
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges(); 

            // Create entries in AppointmentProducts
            for (int i = 0; i < SelectedProductIDs.Count; i++)
            {
                var productId = SelectedProductIDs[i];
                var quantity = SelectedQuantities[i];

                var appointmentProduct = new AppointmentProduct
                {
                    AppointmentID = appointment.Id,
                    ProductID = productId,
                    QuantityLeft = quantity
                };
                _context.AppointmentProducts.Add(appointmentProduct);

                // Subtract quantity from UserProducts
                var userProduct = UserProducts.First(up => up.ProductID == productId);
                userProduct.Quantity -= quantity;
                _context.UserProducts.Update(userProduct);
            }

            _context.SaveChanges();

            return RedirectToPage("Home", new { userId = appointment.UserID });
        }
    }
}
