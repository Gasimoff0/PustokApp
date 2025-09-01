using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class GenreController
        (PustokAppDbContext pustokAppDbContext)
        : Controller
    {
        public IActionResult Index()
        {
            var genres = pustokAppDbContext.Genres.ToList();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            ModelState.Remove("Books");
            
            if (!ModelState.IsValid)
                return View(genre);
                
            if (pustokAppDbContext.Genres.Any(g => g.Name.ToLower() == genre.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "This genre already exists");
                return View(genre); 
            }
            
            pustokAppDbContext.Genres.Add(genre);
            pustokAppDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            
            var genre = pustokAppDbContext.Genres.FirstOrDefault(g => g.Id == id);
            if (genre == null) return NotFound();
            
            return View(genre);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var genre = pustokAppDbContext.Genres.FirstOrDefault(g => g.Id == id);
            if (genre == null) return NotFound();
            
            pustokAppDbContext.Genres.Remove(genre);
            pustokAppDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            
            var genre = pustokAppDbContext.Genres
                .Include(g => g.Books)
                .ThenInclude(b => b.Author)
                .FirstOrDefault(g => g.Id == id);
            
            if (genre == null) return NotFound();
            
            return View(genre);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var genre = pustokAppDbContext.Genres.FirstOrDefault(g => g.Id == id);
            if (genre == null) return NotFound();
            
            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            ModelState.Remove("Books");
            
            if (!ModelState.IsValid)
                return View(genre);
                
            if (pustokAppDbContext.Genres.Any(g => g.Name.ToLower() == genre.Name.ToLower() && g.Id != genre.Id))
            {
                ModelState.AddModelError("Name", "This genre already exists");
                return View(genre);
            }
            
            pustokAppDbContext.Genres.Update(genre);
            pustokAppDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
