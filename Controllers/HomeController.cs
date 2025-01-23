using Microsoft.AspNetCore.Mvc;
using Bazaarly.Data;
using Bazaarly.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Bazaarly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Images) // Ensure images are included
                .ToList();

            return View("Index", products);
        }
        public IActionResult Privacy()
    {
        return View(); // This will return the Privacy.cshtml view
    }


        // Method to get products by category if needed
        public IActionResult GetProductsByCategory(string category)
        {
            var products = _context.Products.Where(p => p.Category == category).ToList();
            return View("Index", products);
        }
        public IActionResult Search(string query)
        {
            // Find the product that matches the search query (e.g., by name)
            var product = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Name.Contains(query)); // Case-insensitive search

            // If no product is found, return to the index page or handle it accordingly
            if (product == null)
            {
                TempData["Message"] = "No product found with that name.";
                return RedirectToAction("Index");
            }

            // Redirect to the product's Details view
            return RedirectToAction("Details", "Product", new { id = product.ProductId });
        }
        public IActionResult ProductsByCategory(string category)
        {
            var products = _context.Products
        .Where(p => p.Category == category)
        .Include(p => p.Images) // Make sure to include images
        .ToList();

            return View(products);
        }

    }
}
