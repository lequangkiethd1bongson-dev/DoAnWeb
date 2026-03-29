using DoAnWeb.Models;
using DoAnWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Admin/Users (Danh sách User chi tiết)
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.UserPhone ?? user.PhoneNumber, // Sử dụng UserPhone từ model mở rộng hoặc PhoneNumber mặc định
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount,
                    Role = roles.FirstOrDefault() ?? "None",
                    IsLocked = await _userManager.IsLockedOutAsync(user)
                });
            }

            return View(userViewModels);
        }

        // POST: Admin/Admin/LockUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Khóa tài khoản trong 100 năm
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            TempData["SuccessMessage"] = $"Đã khóa tài khoản {user.Email}";
            return RedirectToAction("Users");
        }

        // POST: Admin/Admin/UnlockUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["SuccessMessage"] = $"Đã mở khóa tài khoản {user.Email}";
            return RedirectToAction("Users");
        }

        // GET: Admin/Admin/CreateUser
        public IActionResult CreateUser()
        {
            var model = new CreateUserViewModel
            {
                Roles = new List<string> { "Admin", "Staff_Property", "Staff_Appointment", "User" }
            };
            return View(model);
        }

        // POST: Admin/Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email, 
                    FullName = model.FullName,
                    UserPhone = model.PhoneNumber,
                    EmailConfirmed = true 
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["SuccessMessage"] = "Tạo người dùng và gán quyền thành công!";
                    return RedirectToAction("Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Nếu có lỗi, nạp lại danh sách Roles cho View
            model.Roles = new List<string> { "Admin", "Staff_Property", "Staff_Appointment", "User" };
            return View(model);
        }

        // GET: Admin/Admin/EditUserRole/5
        public async Task<IActionResult> EditUserRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var model = new EditUserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                CurrentRole = currentRoles.FirstOrDefault() ?? "None",
                SelectedRole = currentRoles.FirstOrDefault() ?? "User",
                Roles = new List<string> { "Admin", "Staff_Property", "Staff_Appointment", "User" }
            };

            return View(model);
        }

        // POST: Admin/Admin/EditUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRole(EditUserRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null) return NotFound();

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.SelectedRole);

                TempData["SuccessMessage"] = $"Đã cập nhật Role cho {user.Email}";
                return RedirectToAction("Users");
            }

            // Nếu lỗi, load lại list roles
            model.Roles = new List<string> { "Admin", "Staff_Property", "Staff_Appointment", "User" };
            return View(model);
        }

        // POST: Admin/Admin/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa người dùng này.";
            }

            return RedirectToAction("Users");
        }
    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
