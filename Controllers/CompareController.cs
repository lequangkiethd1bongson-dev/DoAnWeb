using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Controllers
{
    public class CompareController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompareController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return View(new List<Property>());
            }

            var idList = ids.Split(',').Select(int.Parse).ToList();
            var properties = await _context.Properties
                .Include(p => p.ImagesProperties)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Where(p => idList.Contains(p.PropertyId))
                .ToListAsync();

            return View(properties);
        }
    }
}
