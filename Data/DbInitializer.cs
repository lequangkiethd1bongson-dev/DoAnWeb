using DoAnWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var _context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Tạo Roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Tạo Admin User
            string adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    UserPhone = "0123456789",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("--> Admin user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"--> Error creating admin: {error.Description}");
                    }
                }
            }
            // 3. Tạo Seed Properties (Nếu chưa có)
            if (await _context.Properties.CountAsync() < 5)
            {
                var properties = new List<Property>
                {
                    new Property
                    {
                        Title = "Căn hộ cao cấp Vinhomes Central Park",
                        Description = "Căn hộ 2 phòng ngủ, đầy đủ tiện nghi, view sông Sài Gòn thoáng mát. Khu dân cư an ninh, nhiều tiện ích như hồ bơi, gym, công viên.",
                        Price = 15000000,
                        Area = 75,
                        Address = "208 Nguyễn Hữu Cảnh, Phường 22",
                        City = "Hồ Chí Minh",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800", IsMain = true },
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800", IsMain = false }
                        }
                    },
                    new Property
                    {
                        Title = "Phòng trọ giá rẻ cho sinh viên Cầu Giấy",
                        Description = "Phòng trọ sạch sẽ, gần các trường đại học lớn như ĐH Quốc Gia, ĐH Sư Phạm. Có lối đi riêng, giờ giấc tự do.",
                        Price = 3500000,
                        Area = 25,
                        Address = "Ngõ 123 Xuân Thủy",
                        City = "Hà Nội",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1560448204-603b3fc33ddc?w=800", IsMain = true },
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800", IsMain = false }
                        }
                    },
                    new Property
                    {
                        Title = "Nhà nguyên căn mặt tiền Đà Nẵng",
                        Description = "Phù hợp kinh doanh hoặc làm văn phòng. Vị trí đắc địa, gần biển Mỹ Khê, giao thông thuận tiện.",
                        Price = 25000000,
                        Area = 120,
                        Address = "Võ Nguyên Giáp, Quận Sơn Trà",
                        City = "Đà Nẵng",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800", IsMain = true },
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=800", IsMain = false }
                        }
                    },
                    new Property
                    {
                        Title = "Studio hiện đại trung tâm Quận 1",
                        Description = "Căn hộ studio đầy đủ nội thất, thiết kế trẻ trung. Ngay trung tâm, thuận tiện di chuyển đến các khu vực vui chơi giải trí.",
                        Price = 12000000,
                        Area = 35,
                        Address = "Lý Tự Trọng, Quận 1",
                        City = "Hồ Chí Minh",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1536376074432-8328d029289b?w=800", IsMain = true }
                        }
                    },
                    new Property
                    {
                        Title = "Biệt thự sân vườn đẳng cấp Đà Lạt",
                        Description = "Không gian yên tĩnh, khí hậu trong lành. Sân vườn rộng rãi, thích hợp nghỉ dưỡng cùng gia đình.",
                        Price = 45000000,
                        Area = 250,
                        Address = "Trần Hưng Đạo, TP Đà Lạt",
                        City = "Lâm Đồng",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1518780664697-55e3ad937233?w=800", IsMain = true }
                        }
                    },
                    new Property
                    {
                        Title = "Căn hộ 3PN Green Bay Mễ Trì",
                        Description = "Diện tích rộng, 3 phòng ngủ phù hợp gia đình đông người. View hồ điều hòa thoáng đãng.",
                        Price = 18000000,
                        Area = 90,
                        Address = "Số 7 Đại lộ Thăng Long",
                        City = "Hà Nội",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=800", IsMain = true }
                        }
                    },
                    new Property
                    {
                        Title = "Phòng trọ cao cấp Tân Bình",
                        Description = "Phòng trọ mới xây, có ban công, gác lửng. Khu phố an ninh, có hầm để xe rộng rãi.",
                        Price = 4500000,
                        Area = 30,
                        Address = "Phan Huy Ích, Quận Tân Bình",
                        City = "Hồ Chí Minh",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1598928506311-c55ded91a20c?w=800", IsMain = true }
                        }
                    },
                    new Property
                    {
                        Title = "Nhà phố cổ Hội An cổ kính",
                        Description = "Nhà cổ được tu sửa giữ nguyên nét truyền thống. Thích hợp kinh doanh homestay hoặc du lịch trải nghiệm.",
                        Price = 30000000,
                        Area = 80,
                        Address = "Trần Phú, Hội An",
                        City = "Quảng Nam",
                        Status = "Available",
                        CreatedAt = DateTime.Now,
                        ImagesProperties = new List<ImagesProperty>
                        {
                            new ImagesProperty { ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800", IsMain = true }
                        }
                    }
                };

                _context.Properties.AddRange(properties);
                await _context.SaveChangesAsync();
                Console.WriteLine("--> Seed properties added successfully.");
            }

            // 4. Tạo Seed Amenities (Nếu chưa có)
            if (!await _context.Amenities.AnyAsync())
            {
                var amenities = new List<Amenity>
                {
                    new Amenity { Name = "Miễn phí Wi-Fi", Icon = "fas fa-wifi" },
                    new Amenity { Name = "Điều hòa nhiệt độ", Icon = "fas fa-snowflake" },
                    new Amenity { Name = "Chỗ để xe", Icon = "fas fa-parking" },
                    new Amenity { Name = "Máy giặt", Icon = "fas fa-tshirt" },
                    new Amenity { Name = "Thang máy", Icon = "fas fa-elevator" },
                    new Amenity { Name = "Camera an ninh", Icon = "fas fa-video" },
                    new Amenity { Name = "Ban công", Icon = "fas fa-sun" },
                    new Amenity { Name = "Bếp riêng", Icon = "fas fa-utensils" }
                };
                _context.Amenities.AddRange(amenities);
                await _context.SaveChangesAsync();
                Console.WriteLine("--> Seed amenities added successfully.");
            }
        }
    }
}
