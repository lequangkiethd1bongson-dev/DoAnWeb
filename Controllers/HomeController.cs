using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DoAnWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? city, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Properties
                .Include(p => p.ImagesProperties)
                .Where(p => p.Status == "Available")
                .AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.City.Contains(city));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var properties = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user != null)
            {
                ViewBag.FavoriteIds = await _context.Favorites
                    .Where(f => f.UserId == user.Id)
                    .Select(f => f.PropertyId)
                    .ToListAsync();
            }
            else
            {
                ViewBag.FavoriteIds = new List<int>();
            }

            ViewBag.City = city;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(properties);
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.ImagesProperties)
                .FirstOrDefaultAsync(m => m.PropertyId == id);

            if (property == null) return NotFound();

            return View(property);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
