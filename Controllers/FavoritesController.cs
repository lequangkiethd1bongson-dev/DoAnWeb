using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var favorites = await _context.Favorites
                .Include(f => f.Property)
                    .ThenInclude(p => p!.ImagesProperties)
                .Where(f => f.UserId == userId)
                .Select(f => f.Property)
                .ToListAsync();
            return View(favorites);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Toggle(int propertyId)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = false });
            }
            else
            {
                _context.Favorites.Add(new Favorite { UserId = userId, PropertyId = propertyId });
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true });
            }
        }
    }
}
