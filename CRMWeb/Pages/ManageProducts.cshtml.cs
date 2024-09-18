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
    public class ManageProductsModel : PageModel
    {
        private readonly CRMContext _context;

        public ManageProductsModel(CRMContext context)
        {
            _context = context;
            Products = new List<Product>();
            UserProducts = new List<UserProduct>();
        }

        // All company products
        public List<Product> Products { get; set; }
        // Products assigned to user
        public List<UserProduct> UserProducts { get; set; }  

        [BindProperty]
        public int ProductID { get; set; }

        [BindProperty]
        public int Quantity { get; set; } = 0;

        public void OnGet()
        {
            // Fetch all products
            Products = _context.Products.ToList();

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

        public IActionResult OnPostAssignProduct()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User ID claim is missing or invalid.");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == ProductID);
            if (product == null || Quantity <= 0)
            {
                ModelState.AddModelError("", "Invalid product selection or quantity.");
                return Page();
            }

            if (product.Quantity < Quantity)
            {
                // Throw error when quantity > available quantity
                ModelState.AddModelError("", $"Requested quantity ({Quantity}) exceeds available quantity ({product.Quantity}).");
                // Fetch products and user products to display
                OnGet();
                return Page();
            }

            // Add or update assigned products
            var userProduct = _context.UserProducts.FirstOrDefault(up => up.UserID == userId && up.ProductID == ProductID);
            if (userProduct == null)
            {
                userProduct = new UserProduct
                {
                    UserID = userId,
                    ProductID = ProductID,
                    Quantity = Quantity
                };
                _context.UserProducts.Add(userProduct);
            }
            else
            {
                userProduct.Quantity += Quantity;
                _context.UserProducts.Update(userProduct);
            }

            // Subtract quantity
            product.Quantity -= Quantity;
            _context.Products.Update(product);

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostRemoveProduct(int userId, int productId)
        {
            // Fetch UserProduct entry by UserID and ProductID
            var userProduct = _context.UserProducts
                .Include(up => up.Product)
                .FirstOrDefault(up => up.UserID == userId && up.ProductID == productId);

            if (userProduct != null)
            {
                // Check if product exists
                var product = userProduct.Product != null
                    ? _context.Products.FirstOrDefault(p => p.Name == userProduct.Product.Name && p.Expiry == userProduct.Product.Expiry)
                    : null;

                if (product != null)
                {
                    // Add quantity to existing product
                    product.Quantity += userProduct.Quantity;
                    _context.Products.Update(product);
                }
                else if (userProduct.Product != null)
                {
                    // If not present - create new entry
                    var newProduct = new Product
                    {
                        Name = userProduct.Product.Name,
                        Expiry = userProduct.Product.Expiry,
                        Quantity = userProduct.Quantity
                    };
                    _context.Products.Add(newProduct);
                }

                // remove userProduct entry
                _context.UserProducts.Remove(userProduct);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

    }
}
