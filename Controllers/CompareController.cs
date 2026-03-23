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
            if (string.IsNullOrWhiteSpace(ids))
            {
                return View(new List<Property>());
            }

            var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => int.TryParse(s, out int id) ? id : 0)
                           .Where(id => id > 0)
                           .ToList();

            if (!idList.Any())
            {
                return View(new List<Property>());
            }

            var properties = new List<Property>();
            foreach (var id in idList)
            {
                var property = await _context.Properties
                    .Include(p => p.ImagesProperties)
                    .Include(p => p.PropertyAmenities)
                        .ThenInclude(pa => pa.Amenity)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.PropertyId == id);
                
                if (property != null)
                {
                    properties.Add(property);
                }
            }

            return View(properties);
        }
    }
}
