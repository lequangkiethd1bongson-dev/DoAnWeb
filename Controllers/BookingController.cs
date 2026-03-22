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

            // 1. Kiểm tra ngày đặt lịch phải là tương lai
            if (appointmentDate <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Ngày hẹn phải là một thời điểm trong tương lai.";
                return RedirectToAction("Details", "Home", new { id = propertyId });
            }

            // 2. Check trùng lịch: Cùng một bất động sản, các cuộc hẹn cách nhau ít nhất 30 phút
            var overlap = _context.Appointments
                .Any(a => a.PropertyId == propertyId && 
                          a.Status != "Cancelled" &&
                          a.AppointmentDate >= appointmentDate.AddMinutes(-30) && 
                          a.AppointmentDate <= appointmentDate.AddMinutes(30));

            if (overlap)
            {
                TempData["ErrorMessage"] = "Xin lỗi, khung giờ này đã có người đặt lịch xem phòng. Vui lòng chọn thời gian khác (cách ít nhất 30 phút).";
                return RedirectToAction("Details", "Home", new { id = propertyId });
            }

            // 3. Tạo lịch hẹn
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

            // 4. Gửi email xác nhận (Mock)
            // SendEmail(user.Email, "Xác nhận đặt lịch", "Lịch hẹn của bạn đã được ghi nhận...");

            TempData["SuccessMessage"] = "Bạn đã đặt lịch thành công! Chúng tôi đã gửi email xác nhận. Vui lòng chờ Admin duyệt.";
            return RedirectToAction("Index", "MyAppointments");
        }
    }
}
