using System.ComponentModel.DataAnnotations;

namespace DoAnWeb.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string Role { get; set; } = "None";
        public bool IsLocked { get; set; }
    }
}
