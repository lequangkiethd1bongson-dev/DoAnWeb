using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWeb.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int propertyId, DateTime appointmentDate, string? note)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var appointment = new Appointment
            {
                PropertyId = propertyId,
                UserId = userId,
                AppointmentDate = appointmentDate,
                Note = note,
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bạn đã đặt lịch thành công! Vui lòng chờ xác nhận.";
            return RedirectToAction("Index", "MyAppointments");
        }
    }
}
