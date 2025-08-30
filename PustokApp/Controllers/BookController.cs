using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{
    public class BookController
        (PustokAppDbContext pustokAppDbContext)
        : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int? id)
        {
            if(id == null) return NotFound();

            var book = pustokAppDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .Include(b => b.Genre)
                .FirstOrDefault(b => b.Id == id);
            if(book == null) return NotFound();
            BookDetailVm bookDetailVm = new()
            {
                Book = book,
                RelatedBooks = pustokAppDbContext.Books
                .Where(b => b.GenreId == book.GenreId && b.Id != book.Id)
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Include(b => b.Genre)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .ToList()
            };
            return View(bookDetailVm);
        }

        public IActionResult BookModal(int? id)
        {
            if (id == null) return NotFound();

            var book = pustokAppDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .Include(b => b.Genre)
                .FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();
            return PartialView("_BookModal",book);
        }
    }
}
