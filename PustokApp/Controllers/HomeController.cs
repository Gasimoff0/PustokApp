using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{
    public class HomeController
        (PustokAppDbContext dbContext)
        : Controller
    {
        public IActionResult Index()
        {
            HomeVm homeVm = new()
            {
                Sliders = dbContext.Sliders.ToList(),
                FeaturedBooks = dbContext.Books
                    .Include(b => b.Author)
                    .Where(b => b.IsFeatured)
                    .ToList(),
                NewBooks = dbContext.Books
                    .Include(b => b.Author)
                    .Where(b => b.IsNew)
                    .ToList(),
                DiscountBooks = dbContext.Books
                    .Include(b => b.Author)
                    .Where(b => b.DiscountPercentage > 0)
                    .ToList()
            };
            return View(homeVm);
        }
    }
}
