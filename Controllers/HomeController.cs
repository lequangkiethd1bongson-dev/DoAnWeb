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

        public async Task<IActionResult> Index(string? keyword, string? city, string? district, string? propertyType, int? rooms, decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, string? sortBy, int? page)
        {
            var query = _context.Properties
                .Include(p => p.ImagesProperties)
                .Include(p => p.Reviews)
                .AsSplitQuery()
                .Where(p => p.Status == "Available")
                .AsQueryable();

            // ... (rest of filtering logic remains same)
            // Lọc theo từ khóa (Tiêu đề hoặc Mô tả)
            if (!string.IsNullOrEmpty(keyword))
            {
                var searchKey = keyword.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(searchKey) || (p.Description != null && p.Description.ToLower().Contains(searchKey)));
            }

            // Lọc theo thành phố hoặc quận/huyện (Vị trí)
            if (!string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(district))
            {
                var searchLoc = (city ?? district ?? "").Trim().ToLower();
                query = query.Where(p => p.City.ToLower().Contains(searchLoc) || p.District.ToLower().Contains(searchLoc));
            }

            // Lọc theo loại bất động sản
            if (!string.IsNullOrEmpty(propertyType))
            {
                query = query.Where(p => p.PropertyType == propertyType);
            }

            // Lọc theo số phòng
            if (rooms.HasValue)
            {
                query = query.Where(p => p.NumberOfRooms >= rooms.Value);
            }

            // Lọc theo giá
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);

            // Lọc theo diện tích
            if (minArea.HasValue) query = query.Where(p => p.Area >= minArea.Value);
            if (maxArea.HasValue) query = query.Where(p => p.Area <= maxArea.Value);

            // Sắp xếp
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt) // Mặc định tin mới nhất
            };

            // Pagination logic
            int pageSize = 6;
            int currentPage = page ?? 1;
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var properties = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var userName = User.Identity?.Name;
            var user = userName != null ? await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName) : null;
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

            // Gửi lại các giá trị lọc để hiển thị trên form và pagination
            ViewBag.Keyword = keyword;
            ViewBag.City = city;
            ViewBag.District = district;
            ViewBag.PropertyType = propertyType;
            ViewBag.Rooms = rooms;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.MinArea = minArea;
            ViewBag.MaxArea = maxArea;
            ViewBag.SortBy = sortBy;
            
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;

            return View(properties);
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.ImagesProperties)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.PropertyId == id);

            if (property == null) return NotFound();

            // Increment View Count
            property.ViewCount++;
            _context.Update(property);
            await _context.SaveChangesAsync();

            return View(property);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
