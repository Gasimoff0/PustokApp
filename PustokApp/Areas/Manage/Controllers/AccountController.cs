using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Areas.Manage.ViewModels;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController
        (UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager
        )
        : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVm adminLoginVm,string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }
            var user = await userManager.FindByNameAsync(adminLoginVm.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }
            if (await userManager.IsInRoleAsync(user, "Member"))
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }
            var result = await userManager.CheckPasswordAsync(user, adminLoginVm.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }
            await signInManager.SignInAsync(user,false);

            return RedirectToAction("index", "dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login");
        }

        public async Task<IActionResult> CreateAdmin()
        {
            AppUser appUser = new AppUser
            {
                FullName = "Admin",
                UserName = "_Admin",
                Email = "admin@gmail.com"
            };
            var result = await userManager.CreateAsync(appUser, "_Admin123");
            await userManager.AddToRoleAsync(appUser, "Admin");
            return Json(result);

        }

    }
}
