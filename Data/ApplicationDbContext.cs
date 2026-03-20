using DoAnWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<ImagesProperty> ImagesProperties { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình quan hệ nếu cần (ví giá trị mặc định của EF thường đã đủ cho các quan hệ 1-N cơ bản)
            
            builder.Entity<Property>()
                .HasMany(p => p.ImagesProperties)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId);

            builder.Entity<Property>()
                .HasMany(p => p.Appointments)
                .WithOne(a => a.Property)
                .HasForeignKey(a => a.PropertyId);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Appointments)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
        }
    }
}
