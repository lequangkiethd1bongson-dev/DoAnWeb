using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWeb.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int PropertyId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }
    }
}
