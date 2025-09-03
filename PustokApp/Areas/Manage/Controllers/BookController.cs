using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class BookController
        (PustokAppDbContext pustokAppDbContext, IWebHostEnvironment webHostEnvironment)
        : Controller
    {
        public IActionResult Index()
        {
            var books = pustokAppDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .OrderBy(b => b.Title)
                .ToList();
            return View(books);
        }

        public IActionResult Create()
        {
            ViewBag.Authors = new SelectList(pustokAppDbContext.Authors.ToList(), "Id", "Name");
            ViewBag.Genres = new SelectList(pustokAppDbContext.Genres.ToList(), "Id", "Name");
            ViewBag.Tags = pustokAppDbContext.Tags.ToList(); 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book, IFormFile? MainImageFile, IFormFile? HoverImageFile, List<IFormFile>? AdditionalImages, List<int>? SelectedTagIds)
        {
            if (MainImageFile == null)
            {
                ModelState.AddModelError("MainImageFile", "Please select a main image");
            }
            else if (!MainImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("MainImageFile", "Please select a valid image file for main image");
            }

            if (HoverImageFile == null)
            {
                ModelState.AddModelError("HoverImageFile", "Please select a hover image");
            }
            else if (!HoverImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("HoverImageFile", "Please select a valid image file for hover image");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = new SelectList(pustokAppDbContext.Authors.ToList(), "Id", "Name");
                ViewBag.Genres = new SelectList(pustokAppDbContext.Genres.ToList(), "Id", "Name");
                ViewBag.Tags = pustokAppDbContext.Tags.ToList(); 
                return View(book);
            }

            if (pustokAppDbContext.Books.Any(b => b.Title.ToLower() == book.Title.ToLower()))
            {
                ModelState.AddModelError("Title", "This book title already exists");
                ViewBag.Authors = new SelectList(pustokAppDbContext.Authors.ToList(), "Id", "Name");
                ViewBag.Genres = new SelectList(pustokAppDbContext.Genres.ToList(), "Id", "Name");
                ViewBag.Tags = pustokAppDbContext.Tags.ToList();
                return View(book);
            }

            if (MainImageFile != null)
            {
                string mainFileName = Guid.NewGuid() + Path.GetExtension(MainImageFile.FileName);
                string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products");
                
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                string mainFilePath = Path.Combine(uploadsPath, mainFileName);
                using (var stream = new FileStream(mainFilePath, FileMode.Create))
                {
                    await MainImageFile.CopyToAsync(stream);
                }
                book.MainImageUrl = mainFileName;
            }

            if (HoverImageFile != null)
            {
                string hoverFileName = Guid.NewGuid() + Path.GetExtension(HoverImageFile.FileName);
                string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products");
                
                string hoverFilePath = Path.Combine(uploadsPath, hoverFileName);
                using (var stream = new FileStream(hoverFilePath, FileMode.Create))
                {
                    await HoverImageFile.CopyToAsync(stream);
                }
                book.HoverImageUrl = hoverFileName;
            }

            if (string.IsNullOrEmpty(book.Code))
            {
                book.Code = "BK" + DateTime.Now.Ticks.ToString().Substring(10);
            }

            book.CreatedAt = DateTime.Now;
            pustokAppDbContext.Books.Add(book);
            await pustokAppDbContext.SaveChangesAsync();

            if (SelectedTagIds != null && SelectedTagIds.Any())
            {
                foreach (var tagId in SelectedTagIds)
                {
                    var bookTag = new BookTag
                    {
                        BookId = book.Id,
                        TagId = tagId
                    };
                    pustokAppDbContext.BookTags.Add(bookTag);
                }
                await pustokAppDbContext.SaveChangesAsync();
            }

            if (AdditionalImages != null && AdditionalImages.Any())
            {
                foreach (var image in AdditionalImages.Where(img => img != null && img.Length > 0))
                {
                    if (image.ContentType.StartsWith("image/"))
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products");
                        string filePath = Path.Combine(uploadsPath, fileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var bookImage = new BookImage
                        {
                            ImageUrl = fileName,
                            BookId = book.Id
                        };
                        pustokAppDbContext.BookImages.Add(bookImage);
                    }
                }
                await pustokAppDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = pustokAppDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var book = pustokAppDbContext.Books
                .Include(b => b.BookImages)
                .FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            if (!string.IsNullOrEmpty(book.MainImageUrl))
            {
                string mainImagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products", book.MainImageUrl);
                if (System.IO.File.Exists(mainImagePath))
                {
                    System.IO.File.Delete(mainImagePath);
                }
            }

            if (!string.IsNullOrEmpty(book.HoverImageUrl))
            {
                string hoverImagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products", book.HoverImageUrl);
                if (System.IO.File.Exists(hoverImagePath))
                {
                    System.IO.File.Delete(hoverImagePath);
                }
            }

            if (book.BookImages != null)
            {
                foreach (var bookImage in book.BookImages)
                {
                    string imagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products", bookImage.ImageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            pustokAppDbContext.Books.Remove(book);
            await pustokAppDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();

            var book = pustokAppDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = pustokAppDbContext.Books
                .Include(b => b.BookImages)
                .Include(b => b.BookTags)
                .FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            ViewBag.Authors = new SelectList(pustokAppDbContext.Authors.ToList(), "Id", "Name", book.AuthorId);
            ViewBag.Genres = new SelectList(pustokAppDbContext.Genres.ToList(), "Id", "Name", book.GenreId);
            ViewBag.Tags = pustokAppDbContext.Tags.ToList();
            ViewBag.SelectedTagIds = book.BookTags?.Select(bt => bt.TagId).ToList() ?? new List<int>();
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Book book, IFormFile? MainImageFile, IFormFile? HoverImageFile, List<IFormFile>? AdditionalImages, List<int>? SelectedTagIds)
        {
            var existingBook = pustokAppDbContext.Books
                .Include(b => b.BookImages)
                .Include(b => b.BookTags)
                .FirstOrDefault(b => b.Id == book.Id);
            if (existingBook == null) return NotFound();

            if (MainImageFile != null && !MainImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("MainImageFile", "Please select a valid image file for main image");
            }

            if (HoverImageFile != null && !HoverImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("HoverImageFile", "Please select a valid image file for hover image");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = new SelectList(pustokAppDbContext.Authors.ToList(), "Id", "Name", book.AuthorId);
                ViewBag.Genres = new SelectList(pustokAppDbContext.Genres.ToList(), "Id", "Name", book.GenreId);
                ViewBag.Tags = pustokAppDbContext.Tags.ToList();
                ViewBag.SelectedTagIds = SelectedTagIds ?? new List<int>();
                return View(book);
            }

            if (pustokAppDbContext.Books.Any(b => b.Title.ToLower() == book.Title.ToLower() && b.Id != book.Id))
            {
                ModelState.AddModelError("Title", "This book title already exists");
                ViewBag.Authors = new SelectList(pustokAppDbContext.Authors.ToList(), "Id", "Name", book.AuthorId);
                ViewBag.Genres = new SelectList(pustokAppDbContext.Genres.ToList(), "Id", "Name", book.GenreId);
                ViewBag.Tags = pustokAppDbContext.Tags.ToList();
                ViewBag.SelectedTagIds = SelectedTagIds ?? new List<int>();
                return View(book);
            }

            if (MainImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingBook.MainImageUrl))
                {
                    string oldMainImagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products", existingBook.MainImageUrl);
                    if (System.IO.File.Exists(oldMainImagePath))
                    {
                        System.IO.File.Delete(oldMainImagePath);
                    }
                }

                string mainFileName = Guid.NewGuid() + Path.GetExtension(MainImageFile.FileName);
                string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products");
                
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                string mainFilePath = Path.Combine(uploadsPath, mainFileName);
                using (var stream = new FileStream(mainFilePath, FileMode.Create))
                {
                    await MainImageFile.CopyToAsync(stream);
                }
                existingBook.MainImageUrl = mainFileName;
            }

            if (HoverImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingBook.HoverImageUrl))
                {
                    string oldHoverImagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products", existingBook.HoverImageUrl);
                    if (System.IO.File.Exists(oldHoverImagePath))
                    {
                        System.IO.File.Delete(oldHoverImagePath);
                    }
                }

                string hoverFileName = Guid.NewGuid() + Path.GetExtension(HoverImageFile.FileName);
                string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products");
                
                string hoverFilePath = Path.Combine(uploadsPath, hoverFileName);
                using (var stream = new FileStream(hoverFilePath, FileMode.Create))
                {
                    await HoverImageFile.CopyToAsync(stream);
                }
                existingBook.HoverImageUrl = hoverFileName;
            }

            existingBook.Title = book.Title;
            existingBook.Description = book.Description;
            existingBook.Price = book.Price;
            existingBook.DiscountPercentage = book.DiscountPercentage;
            existingBook.IsFeatured = book.IsFeatured;
            existingBook.IsNew = book.IsNew;
            existingBook.InStock = book.InStock;
            existingBook.Code = book.Code;
            existingBook.AuthorId = book.AuthorId;
            existingBook.GenreId = book.GenreId;
            existingBook.UpdatedAt = DateTime.Now;

            if (existingBook.BookTags != null)
            {
                pustokAppDbContext.BookTags.RemoveRange(existingBook.BookTags);
            }

            if (SelectedTagIds != null && SelectedTagIds.Any())
            {
                foreach (var tagId in SelectedTagIds)
                {
                    var bookTag = new BookTag
                    {
                        BookId = book.Id,
                        TagId = tagId
                    };
                    pustokAppDbContext.BookTags.Add(bookTag);
                }
            }

            if (AdditionalImages != null && AdditionalImages.Any())
            {
                foreach (var image in AdditionalImages.Where(img => img != null && img.Length > 0))
                {
                    if (image.ContentType.StartsWith("image/"))
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        string uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products");
                        string filePath = Path.Combine(uploadsPath, fileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var bookImage = new BookImage
                        {
                            ImageUrl = fileName,
                            BookId = book.Id
                        };
                        pustokAppDbContext.BookImages.Add(bookImage);
                    }
                }
            }

            await pustokAppDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBookImage(int id)
        {
            var bookImage = pustokAppDbContext.BookImages.FirstOrDefault(bi => bi.Id == id);
            if (bookImage == null) return NotFound();

            string imagePath = Path.Combine(webHostEnvironment.WebRootPath, "assets", "image", "products", bookImage.ImageUrl);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            pustokAppDbContext.BookImages.Remove(bookImage);
            await pustokAppDbContext.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}