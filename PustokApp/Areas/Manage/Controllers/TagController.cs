using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TagController
        (PustokAppDbContext pustokAppDbContext)
        : Controller
    {
        public IActionResult Index()
        {
            var tags = pustokAppDbContext.Tags
                .Include(t => t.BookTags)
                .ThenInclude(bt => bt.Book)
                .OrderBy(t => t.Name)
                .ToList();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid)
                return View(tag);
                
            if (pustokAppDbContext.Tags.Any(t => t.Name.ToLower() == tag.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "This tag already exists");
                return View(tag); 
            }
            
            pustokAppDbContext.Tags.Add(tag);
            pustokAppDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            
            var tag = pustokAppDbContext.Tags
                .Include(t => t.BookTags)
                .ThenInclude(bt => bt.Book)
                .FirstOrDefault(t => t.Id == id);
            if (tag == null) return NotFound();
            
            return View(tag);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var tag = pustokAppDbContext.Tags
                .Include(t => t.BookTags)
                .FirstOrDefault(t => t.Id == id);
            if (tag == null) return NotFound();
            
            if (tag.BookTags != null && tag.BookTags.Any())
            {
                pustokAppDbContext.BookTags.RemoveRange(tag.BookTags);
            }
            
            pustokAppDbContext.Tags.Remove(tag);
            pustokAppDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            
            var tag = pustokAppDbContext.Tags
                .Include(t => t.BookTags)
                .ThenInclude(bt => bt.Book)
                .ThenInclude(b => b.Author)
                .Include(t => t.BookTags)
                .ThenInclude(bt => bt.Book)
                .ThenInclude(b => b.Genre)
                .FirstOrDefault(t => t.Id == id);
            
            if (tag == null) return NotFound();
            
            return View(tag);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var tag = pustokAppDbContext.Tags.FirstOrDefault(t => t.Id == id);
            if (tag == null) return NotFound();
            
            return View(tag);
        }

        [HttpPost]
        public IActionResult Edit(Tag tag)
        {
            if (!ModelState.IsValid)
                return View(tag);
                
            if (pustokAppDbContext.Tags.Any(t => t.Name.ToLower() == tag.Name.ToLower() && t.Id != tag.Id))
            {
                ModelState.AddModelError("Name", "This tag already exists");
                return View(tag);
            }
            
            pustokAppDbContext.Tags.Update(tag);
            pustokAppDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}