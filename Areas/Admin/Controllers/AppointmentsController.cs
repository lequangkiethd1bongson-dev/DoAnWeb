using DoAnWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Appointments
        public async Task<IActionResult> Index(DateTime? date, string? search)
        {
            var query = _context.Appointments
                .Include(a => a.Property)
                .Include(a => a.User)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchKey = search.Trim().ToLower();
                query = query.Where(a => a.User!.FullName!.ToLower().Contains(searchKey) || 
                                         a.Property!.Title!.ToLower().Contains(searchKey));
            }

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            ViewBag.DateFilter = date?.ToString("yyyy-MM-dd");
            ViewBag.SearchFilter = search;

            return View(appointments);
        }

        // GET: Admin/Appointments/ExportToCsv
        public async Task<IActionResult> ExportToCsv(DateTime? date, string? search)
        {
            var query = _context.Appointments
                .Include(a => a.Property)
                .Include(a => a.User)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchKey = search.Trim().ToLower();
                query = query.Where(a => a.User!.FullName!.ToLower().Contains(searchKey) || 
                                         a.Property!.Title!.ToLower().Contains(searchKey));
            }

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("ID,Khách hàng,Bất động sản,Ngày hẹn,Trạng thái,Ghi chú");

            foreach (var app in appointments)
            {
                builder.AppendLine($"{app.AppointmentId},\"{app.User?.FullName}\",\"{app.Property?.Title}\",\"{app.AppointmentDate:yyyy-MM-dd HH:mm}\",\"{app.Status}\",\"{app.Note?.Replace("\"", "\"\"")}\"");
            }

            var csvData = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(builder.ToString())).ToArray();
            return File(csvData, "text/csv", $"Appointments_{DateTime.Now:yyyyMMdd}.csv");
        }

        // POST: Admin/Appointments/Approve/5
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Property)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment != null)
            {
                appointment.Status = "Approved";
                
                // Cập nhật trạng thái bất động sản sang "Sold" (Đã cho thuê/bán)
                if (appointment.Property != null)
                {
                    appointment.Property.Status = "Sold";
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Appointments/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
