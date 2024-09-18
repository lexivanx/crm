using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRMWeb.Data;
using System.Security.Claims;

namespace CRMWeb.Pages
{
    public class LoginModel : PageModel
    {
        private readonly CRMContext _context;

        public LoginModel(CRMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate credentials
            var user = _context.Users.SingleOrDefault(u => u.Username == Username && u.Password == Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return Page();
            }

            // Create claims for logged-in user
            var claims = new List<Claim>
            // Store username and ID in claims
            {
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Remember user
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, 
            };

            // Sign in
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToPage("/Home");
        }
    }
}
