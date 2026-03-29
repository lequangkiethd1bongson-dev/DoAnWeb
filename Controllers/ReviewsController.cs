using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnWeb.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    review.CreatedAt = DateTime.Now;
                    review.IsApproved = false; // Luôn mặc định là false khi mới tạo
                    _context.Reviews.Add(review);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá! Đánh giá sẽ hiển thị sau khi được kiểm duyệt.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi đánh giá: " + ex.Message;
                }
            }
            else
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + errors;
            }
            return RedirectToAction("Details", "Home", new { id = review.PropertyId });
        }
    }
}
