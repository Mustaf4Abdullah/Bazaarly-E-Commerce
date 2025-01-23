using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bazaarly.Data;
using Bazaarly.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Bazaarly.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment; // For the images
        private readonly UserManager<IdentityUser> _userManager; // UserManager for IdentityUser

        public ManagerController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager; // Injecting UserManager
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ViewEmployees()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Save the product
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Handle the image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Create the image record
                    var image = new Image
                    {
                        ImageUrl = "/images/" + fileName,
                        ProductId = product.ProductId
                    };

                    _context.Images.Add(image);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("ManageProducts"); // Redirect to the Manage Products page
            }
            return View(product);
        }

        public IActionResult ManageProducts()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult Messages()
        {
            // Logic to view messages from employees and reports from customers
            return View();
        }

        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.ProductId == id); // Include images
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile[] images) // Update parameter to handle multiple images
        {
            if (ModelState.IsValid)
            {
                // Update the product details
                _context.Products.Update(product);
                await _context.SaveChangesAsync(); // Save the product first

                // Handle new image uploads
                if (images != null && images.Length > 0)
                {
                    foreach (var imageFile in images)
                    {
                        if (imageFile.Length > 0)
                        {
                            var fileName = Path.GetFileName(imageFile.FileName);
                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(stream);
                            }

                            // Create a new image record
                            var image = new Image
                            {
                                ImageUrl = "/images/" + fileName,
                                ProductId = product.ProductId
                            };

                            _context.Images.Add(image);
                        }
                    }
                    await _context.SaveChangesAsync(); // Save changes for images
                }

                return RedirectToAction("ManageProducts"); // Redirect to Manage Products
            }
            return View(product);
        }

        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageProducts"); // Redirect to Manage Products
        }

        public IActionResult ViewUsers()
        {
            var users = _userManager.Users.ToList(); // Use UserManager to fetch all users
            return View(users); // Return the users to the View
        }

        public IActionResult ViewOrders()
        {
            var users = _userManager.Users.ToList(); // Fetch users once

            var orders = _context.Orders
                .Include(o => o.OrderItems) // Include related OrderItems
                .ThenInclude(oi => oi.Product) // Include related Product details for each OrderItem
                .ToList(); // Execute the query and materialize the data first

            // Loop over the orders to set the additional properties
            foreach (var order in orders)
            {
                // Set CustomerName from UserId
                var user = users.FirstOrDefault(u => u.Id == order.UserId);
                order.CustomerName = user?.UserName ?? "Unknown";

                // Set the order status (modify the logic according to your needs)
                order.Status = "Pending"; // Replace with real logic if needed
            }

            return View(orders); // Return the list of orders to the view
        }


    }
}
