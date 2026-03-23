using DoAnWeb.Data;
using DoAnWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PropertiesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/Properties
        public async Task<IActionResult> Index()
        {
            var properties = await _context.Properties
                .Include(p => p.ImagesProperties)
                .ToListAsync();
            return View(properties);
        }

        // GET: Admin/Properties/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Amenities = await _context.Amenities.ToListAsync();
            return View();
        }

        // POST: Admin/Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, List<IFormFile> imageFiles, int mainImageIndex, int[] selectedAmenities)
        {
            if (ModelState.IsValid)
            {
                // Add Amenities
                if (selectedAmenities != null)
                {
                    property.PropertyAmenities = new List<PropertyAmenity>();
                    foreach (var amenityId in selectedAmenities)
                    {
                        property.PropertyAmenities.Add(new PropertyAmenity { AmenityId = amenityId });
                    }
                }

                _context.Add(property);
                await _context.SaveChangesAsync();

                if (imageFiles != null && imageFiles.Count > 0)
                {
                    string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "properties");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    for (int i = 0; i < imageFiles.Count; i++)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFiles[i].FileName);
                        string filePath = Path.Combine(uploadDir, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFiles[i].CopyToAsync(fileStream);
                        }

                        var imageProperty = new ImagesProperty
                        {
                            PropertyId = property.PropertyId,
                            ImageUrl = "/uploads/properties/" + fileName,
                            IsMain = (i == mainImageIndex)
                        };
                        _context.Add(imageProperty);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(property);
        }

        // GET: Admin/Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var property = await _context.Properties
                .Include(p => p.ImagesProperties)
                .Include(p => p.PropertyAmenities)
                .FirstOrDefaultAsync(m => m.PropertyId == id);

            if (property == null) return NotFound();

            ViewBag.Amenities = await _context.Amenities.ToListAsync();
            ViewBag.SelectedAmenities = property.PropertyAmenities.Select(pa => pa.AmenityId).ToList();

            return View(property);
        }

        // POST: Admin/Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property, List<IFormFile> imageFiles, int mainImageIndex, int[] selectedAmenities)
        {
            if (id != property.PropertyId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Update Amenities
                    var existingAmenities = _context.PropertyAmenities.Where(pa => pa.PropertyId == id);
                    _context.PropertyAmenities.RemoveRange(existingAmenities);

                    if (selectedAmenities != null)
                    {
                        foreach (var amenityId in selectedAmenities)
                        {
                            _context.PropertyAmenities.Add(new PropertyAmenity { PropertyId = id, AmenityId = amenityId });
                        }
                    }

                    _context.Update(property);
                    await _context.SaveChangesAsync();

                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        // Xử lý thêm ảnh mới nếu có
                        string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "properties");
                        
                        // Nếu người dùng chọn ảnh chính mới từ list ảnh mới
                        // (Phần này có thể làm phức tạp hơn để đổi IsMain của ảnh cũ, ở đây tôi làm đơn giản là thêm mới)
                        
                        for (int i = 0; i < imageFiles.Count; i++)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFiles[i].FileName);
                            string filePath = Path.Combine(uploadDir, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFiles[i].CopyToAsync(fileStream);
                            }

                            var imageProperty = new ImagesProperty
                            {
                                PropertyId = property.PropertyId,
                                ImageUrl = "/uploads/properties/" + fileName,
                                IsMain = false // Mặc định false, Admin có thể chỉnh sửa IsMain sau hoặc ở view chi tiết
                            };
                            _context.Add(imageProperty);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(property.PropertyId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(property);
        }

        // GET: Admin/Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var property = await _context.Properties
                .Include(p => p.ImagesProperties)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(m => m.PropertyId == id);

            if (property == null) return NotFound();

            return View(property);
        }

        // GET: Admin/Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var property = await _context.Properties
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (property == null) return NotFound();

            return View(property);
        }

        // POST: Admin/Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties
                .Include(p => p.ImagesProperties)
                .Include(p => p.PropertyAmenities)
                .Include(p => p.Appointments)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property != null)
            {
                // Xóa các file ảnh vật lý
                foreach (var image in property.ImagesProperties)
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            property.Status = status;
            _context.Update(property);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.ImagesProperties.FindAsync(id);
            if (image == null) return NotFound();

            // Xóa file vật lý
            string filePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Xóa khỏi DB
            _context.ImagesProperties.Remove(image);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.PropertyId == id);
        }
    }
}
