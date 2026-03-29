using System.ComponentModel.DataAnnotations;

namespace DoAnWeb.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Mật khẩu phải từ {2} ký tự trở lên.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        public List<string> Roles { get; set; } = new List<string>();
    }
}
