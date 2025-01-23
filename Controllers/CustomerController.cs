using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bazaarly.Data;
using Bazaarly.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace Bazaarly.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var products = _context.Products
                .Include(p => p.Images) // Ensure images are included
                .ToList();
            var userId = User.Identity.Name;
            var cartItems = _context.CartItems.Where(c => c.UserId == userId).ToList();

            ViewBag.CartItems = cartItems;

            return View(products);
        }

        public IActionResult Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the user's unique ID
            var cartItems = _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)  // Eager load the Product navigation property
                .ToList();

            return View(cartItems);
        }

        public IActionResult Favorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the user's unique ID
            var favorites = _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Product)  // Eager load the Product navigation property
                .ToList();

            return View(favorites);
        }


        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the user's unique ID
            var product = _context.Products.Find(productId);

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = product.ProductId,
                Quantity = 1
            };

            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Cart");
        }


        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the user's unique ID

            var existingFavorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (existingFavorite == null)
            {
                // If not, create a new favorite item
                var newFavorite = new Favorites
                {
                    ProductId = productId,
                    UserId = userId
                };

                _context.Favorites.Add(newFavorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        public IActionResult RemoveFromFavorites(int favoriteId)
        {
            var favorite = _context.Favorites.Find(favoriteId);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                _context.SaveChanges();
            }

            return RedirectToAction("Favorites");
        }

        public IActionResult UsersAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            var orders = _context.Orders.Where(o => o.UserId == userId).ToList();
            var addresses = _context.Addresses.Where(a => a.UserId == userId).ToList();  // Fetch addresses

            var viewModel = new UserAccountViewModel
            {
                UserName = user?.UserName,
                Email = user?.Email,
                OrderHistory = orders.Select(o => new Order
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                }).ToList(),
                SavedAddresses = addresses  // Include addresses in the view model
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult AddAddress()
        {
            return View(new Addresses());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAddress(Addresses address)
        {
            if (ModelState.IsValid)
            {
                // Use the user's ID, not their username
                address.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Correctly set the user ID
                _context.Addresses.Add(address);
                _context.SaveChanges();
                return RedirectToAction("UsersAccount"); // Redirect to account page after adding address
            }
            return View(address); // Return the form if validation fails
        }

        // GET: Account/EditProfile
        public IActionResult EditProfile()
        {
            var userId = User.Identity.Name; // Get the logged-in user's username
            var user = _context.Users.FirstOrDefault(u => u.UserName == userId); // Get user info

            // Check if user is found
            if (user == null)
            {
                return NotFound(); // Handle user not found
            }

            var model = new UserAccountViewModel
            {
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model);
        }

        // POST: Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(UserAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name; // Get the logged-in user's username
                var user = _context.Users.FirstOrDefault(u => u.UserName == userId); // Get user info

                // Check if user is found
                if (user == null)
                {
                    return NotFound(); // Handle user not found
                }

                // Update user properties
                user.UserName = model.UserName;
                user.Email = model.Email;

                // Save changes to the database
                _context.Users.Update(user);
                _context.SaveChanges();

                return RedirectToAction("UsersAccount", "Customer");
            }

            return View("EditProfile", model);
        }




        // GET: /Customer/EditAddress?addressId=3
        [HttpGet]
        public IActionResult EditAddress(int addressId)
        {
            var address = _context.Addresses.Find(addressId);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        // POST: /Customer/EditAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAddress(Addresses address)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing address from the database
                var existingAddress = _context.Addresses.Find(address.AddressId);
                if (existingAddress != null)
                {
                    // Update the existing address properties
                    existingAddress.Street = address.Street;
                    existingAddress.City = address.City;
                    existingAddress.State = address.State;
                    existingAddress.ZipCode = address.ZipCode;
                    existingAddress.Country = address.Country;

                    // Save changes to the database
                    _context.SaveChanges();
                    return RedirectToAction("UsersAccount"); // Redirect to account page after updating
                }
                return NotFound(); // If the address does not exist
            }
            return View(address); // Return the form if validation fails
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(int addressId)
        {
            var address = _context.Addresses.Find(addressId);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                _context.SaveChanges();
            }
            return RedirectToAction("UsersAccount"); // Redirect after deletion
        }
        public IActionResult Checkout()
        {
            // Get the user's ID, not their username
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the cart items for the user
            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            // Get user addresses
            var addresses = _context.Addresses
                .Where(a => a.UserId == userId)
                .ToList();

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                Addresses = addresses
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(int selectedAddressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the cart items for the user, including the product details
            var cartItems = _context.CartItems
                .Include(c => c.Product) // Ensure Product details are included
                .Where(c => c.UserId == userId)
                .ToList();

            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Checkout"); // Redirect back to checkout if the cart is empty
            }

            // Retrieve customer information, e.g., customer name (pseudo-code)
            var customerName = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.UserName) // Assuming you have a FullName property in your User model
                .FirstOrDefault();

            // Create a new order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                AddressId = selectedAddressId, // This links to the Addresses table
                Addresses = _context.Addresses.Find(selectedAddressId), // This retrieves the actual address
                CustomerName = customerName, // Set the customer name
                Status = "Pending", // Default status
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId, // Directly assign ProductId from CartItem
                    Quantity = item.Quantity // Keep the Quantity from CartItem
                }).ToList(),
                TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity) // Calculate total amount here

            };


            // Add the order to the context and save
            _context.Orders.Add(order);

            
             _context.SaveChanges(); // Save changes here
            
            

            // Clear the cart after placing the order
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            // Set a success message
            TempData["SuccessMessage"] = "Your order has been placed successfully!";

            return RedirectToAction("Dashboard", "Customer"); // Redirect to the dashboard
        }

        public IActionResult OrderDetails(int orderId)
        {
            // Fetch the order including its items and address
            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Include product details
                .Include(o => o.Addresses) // Include address details
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order); // Pass the Order object directly to the view
        }

        [HttpPost]
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
                .Include(p => p.Images) // Ensure images are included
                .Where(p => p.Category == category) // Filter products by the string category
                .ToList();

            ViewData["Title"] = category; // Set the title based on the category

            return View(products);
        }





    }
}
