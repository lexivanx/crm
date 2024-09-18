using Microsoft.AspNetCore.Mvc.RazorPages;
using CRMWeb.Data;
using CRMWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRMWeb.Pages
{
    [Authorize]
    public class AnalyticsModel : PageModel
    {
        private readonly CRMContext _context;
        private readonly AnalyticsService _analyticsService;

        public AnalyticsModel(CRMContext context, AnalyticsService analyticsService)
        {
            _context = context;
            _analyticsService = analyticsService;

            ProductSampleDistribution = new Dictionary<string, int>();
            DoctorVisitDistribution = new Dictionary<string, int>();
        }

        public Dictionary<string, int> ProductSampleDistribution { get; set; }
        public Dictionary<string, int> DoctorVisitDistribution { get; set; }

        // Fetch data for charts when loading page using helper methods in AnalyticsService
        public void OnGet()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var appointments = _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.AppointmentProducts!)
                    .ThenInclude(ap => ap.Product!)
                    .Where(a => a.UserID == userId)
                    .ToList();

                ProductSampleDistribution = _analyticsService.GetProductSampleDistribution(appointments);
                DoctorVisitDistribution = _analyticsService.GetDoctorVisitDistribution(appointments);
            }
        }
    }
}
