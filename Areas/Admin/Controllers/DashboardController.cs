using DoAnWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Tổng số bất động sản
            var totalProperties = await _context.Properties.CountAsync();

            // 2. Tổng số lịch hẹn
            var totalAppointments = await _context.Appointments.CountAsync();

            // 3. Số lịch hẹn chờ duyệt
            var pendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending");

            // 4. Bất động sản "Hot" nhất (nhiều lượt yêu thích nhất)
            var hotProperty = await _context.Favorites
                .GroupBy(f => f.PropertyId)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .FirstOrDefaultAsync();
            
            var hotPropertyDetail = hotProperty != null 
                ? await _context.Properties.Include(p => p.ImagesProperties).FirstOrDefaultAsync(p => p.PropertyId == hotProperty.Id)
                : null;

            // 5. Tổng lượt xem toàn trang
            var totalViews = await _context.Properties.SumAsync(p => p.ViewCount);

            // 6. Top Buyer Intent (Top 5 BĐS có tỉ lệ Yêu thích / Lượt xem cao hoặc đơn giản là nhiều Yêu thích nhất)
            var topBuyerIntent = await _context.Favorites
                .GroupBy(f => f.PropertyId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new { 
                    Property = _context.Properties.Include(p => p.ImagesProperties).FirstOrDefault(p => p.PropertyId == g.Key),
                    FavCount = g.Count()
                })
                .ToListAsync();

            // 7. Thống kê lịch hẹn theo ngày (cho biểu đồ)
            var appointmentStats = await _context.Appointments
                .Where(a => a.AppointmentDate >= DateTime.Now.AddDays(-7))
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            ViewBag.TotalProperties = totalProperties;
            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.PendingAppointments = pendingAppointments;
            ViewBag.HotProperty = hotPropertyDetail;
            ViewBag.HotPropertyFavCount = hotProperty?.Count ?? 0;
            ViewBag.TotalViews = totalViews;
            ViewBag.TopBuyerIntent = topBuyerIntent;
            ViewBag.AppointmentStatsLabels = appointmentStats.Select(s => s.Date.ToString("dd/MM")).ToList();
            ViewBag.AppointmentStatsData = appointmentStats.Select(s => s.Count).ToList();

            return View();
        }
    }
}
