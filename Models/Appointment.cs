using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWeb.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int PropertyId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; } = "Pending"; // e.g., Pending, Approved, Cancelled

        public string? Note { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }
    }
}
