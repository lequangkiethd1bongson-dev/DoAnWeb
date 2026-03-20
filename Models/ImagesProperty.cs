using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWeb.Models
{
    public class ImagesProperty
    {
        [Key]
        public int ImageId { get; set; }

        public int PropertyId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }
    }
}
