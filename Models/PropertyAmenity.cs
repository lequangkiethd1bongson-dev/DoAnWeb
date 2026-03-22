using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWeb.Models
{
    public class PropertyAmenity
    {
        [Key]
        public int Id { get; set; }

        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }

        public int AmenityId { get; set; }
        [ForeignKey("AmenityId")]
        public virtual Amenity? Amenity { get; set; }
    }
}
