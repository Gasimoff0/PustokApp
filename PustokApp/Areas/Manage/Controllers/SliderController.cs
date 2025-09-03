using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController
        (PustokAppDbContext pustokAppDbContext, IWebHostEnvironment webHostEnvironment)
        : Controller
    {
        public IActionResult Index()
        {
            var sliders = pustokAppDbContext.Sliders.OrderBy(s => s.Order).ToList();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider slider, IFormFile? ImageFile)
        {
            if (ImageFile != null)
            {
                if (!ImageFile.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("ImageFile", "Please select a valid image file");
                }
                else if (ImageFile.Length > 5 * 1024 * 1024) 
                {
                    ModelState.AddModelError("ImageFile", "Image size must be less than 5MB");
                }
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Please select an image");
            }

            if (!ModelState.IsValid)
                return View(slider);

            if (pustokAppDbContext.Sliders.Any(s => s.Title.ToLower() == slider.Title.ToLower()))
            {
                ModelState.AddModelError("Title", "This slider title already exists");
                return View(slider);
            }

            if (ImageFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "bg-images");
                
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                string filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                slider.ImageUrl = fileName;
            }

            slider.CreatedAt = DateTime.Now;
            pustokAppDbContext.Sliders.Add(slider);
            await pustokAppDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var slider = pustokAppDbContext.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var slider = pustokAppDbContext.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider == null) return NotFound();

            if (!string.IsNullOrEmpty(slider.ImageUrl))
            {
                string imagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "bg-images", slider.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            pustokAppDbContext.Sliders.Remove(slider);
            await pustokAppDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();

            var slider = pustokAppDbContext.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider == null) return NotFound();

            return View(slider);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var slider = pustokAppDbContext.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Slider slider, IFormFile? ImageFile)
        {
            var existingSlider = pustokAppDbContext.Sliders.FirstOrDefault(s => s.Id == slider.Id);
            if (existingSlider == null) return NotFound();

            if (ImageFile != null)
            {
                if (!ImageFile.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("ImageFile", "Please select a valid image file");
                }
                else if (ImageFile.Length > 5 * 1024 * 1024) 
                {
                    ModelState.AddModelError("ImageFile", "Image size must be less than 5MB");
                }
            }

            if (!ModelState.IsValid)
                return View(slider);

            if (pustokAppDbContext.Sliders.Any(s => s.Title.ToLower() == slider.Title.ToLower() && s.Id != slider.Id))
            {
                ModelState.AddModelError("Title", "This slider title already exists");
                return View(slider);
            }

            if (ImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingSlider.ImageUrl))
                {
                    string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "bg-images", existingSlider.ImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "bg-images");
                
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                string filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                existingSlider.ImageUrl = fileName;
            }

            existingSlider.Title = slider.Title;
            existingSlider.Description = slider.Description;
            existingSlider.ButtonText = slider.ButtonText;
            existingSlider.ButtonLink = slider.ButtonLink;
            existingSlider.Order = slider.Order;
            existingSlider.UpdatedAt = DateTime.Now;

            await pustokAppDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
