using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Lấy danh sách những người dùng đã từng nhắn tin hoặc gửi tin nhắn cho admin này
            var chatUsers = await _context.Users
                .Where(u => _context.ChatMessages.Any(m => (m.SenderId == u.Id && m.ReceiverId == adminId) || (m.SenderId == adminId && m.ReceiverId == u.Id)))
                .ToListAsync();

            // Nếu muốn lấy tất cả người dùng (ví dụ admin có thể chủ động chat với ai đó mới)
            // chatUsers = await _context.Users.Where(u => u.Id != adminId).ToListAsync();

            return View(chatUsers);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string userId)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var messages = await _context.ChatMessages
                .Where(m => (m.SenderId == adminId && m.ReceiverId == userId) || (m.SenderId == userId && m.ReceiverId == adminId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    m.Id,
                    m.SenderId,
                    m.ReceiverId,
                    Content = m.Content,
                    m.Timestamp,
                    IsAdminSender = m.SenderId == adminId
                })
                .ToListAsync();

            return Json(messages);
        }
    }
}
