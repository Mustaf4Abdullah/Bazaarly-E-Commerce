using Microsoft.AspNetCore.Mvc;
using Bazaarly.Data;
using Bazaarly.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Bazaarly.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompositeViewEngine _viewEngine;

        public ProductController(ApplicationDbContext context, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _viewEngine = viewEngine;
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult AddComment(Comments comment)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                // Return error response if the model is invalid
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Invalid comment data", errors });
            }

            // Check if the product exists
            var product = _context.Products.FirstOrDefault(p => p.ProductId == comment.ProductId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            // Set the DatePosted and save the comment
            comment.DatePosted = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();

            // Return the new comment directly as a JSON response
            return RedirectToAction("Details", new { id = comment.ProductId });
        }
        [HttpPost]
        [Authorize(Roles = "Manager")] // Only allow managers to access this action
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Product", new { id = comment.ProductId });
        }
        [HttpPost]
        public IActionResult ReportProduct(int productId, string reason, string details)
        {
            // Create a new report object
            var report = new Report
            {
                ProductId = productId,
                Reason = reason,
                Details = details,
                DateReported = DateTime.Now
            };

            // Save the report to the database (assuming you have a _context for the reports)
            _context.Reports.Add(report);
            _context.SaveChanges();

            // Return a success response
            return RedirectToAction("Details");
        }







    }
}