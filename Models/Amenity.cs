using System.ComponentModel.DataAnnotations;

namespace DoAnWeb.Models
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Icon { get; set; } // FontAwesome class, e.g., "fas fa-wifi"

        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    }
}
