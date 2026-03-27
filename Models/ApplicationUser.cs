using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DoAnWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        public string? UserPhone { get; set; } // Map thêm trường Phone theo yêu cầu
        
        public string? AvatarUrl { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
