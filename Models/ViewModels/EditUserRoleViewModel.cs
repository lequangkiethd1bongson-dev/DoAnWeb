using System.ComponentModel.DataAnnotations;

namespace DoAnWeb.Models.ViewModels
{
    public class EditUserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SelectedRole { get; set; } = string.Empty;

        public string CurrentRole { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string>();
    }
}
