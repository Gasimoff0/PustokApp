using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
                Sliders = dbContext.Sliders.ToList()
            };
            return View(homeVm);
        }
    }
}
