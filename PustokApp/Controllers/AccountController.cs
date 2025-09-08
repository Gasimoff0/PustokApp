using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using PustokApp.Models;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{
    public class AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager) : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVm userLoginVm, string ReturnUrl)
        {
            var user = await userManager.FindByNameAsync(userLoginVm.UserNameOrEmail);
            if (user == null)
            {
                user = await userManager.FindByEmailAsync(userLoginVm.UserNameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("", "Username or password is incorrect");
                    return View(userLoginVm);
                }
            }
            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(userLoginVm);
            }
            var result = await signInManager.PasswordSignInAsync(user, userLoginVm.Password, userLoginVm.RememberMe, true);
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please confirm your email address.");
                return View(userLoginVm);
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is blocked.");
                return View(userLoginVm);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(userLoginVm);
            }

            if (ReturnUrl is null) return RedirectToAction("Index", "Home");

            return Redirect(ReturnUrl);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVm userRegisterVm)
        {
            if (!ModelState.IsValid)
                return View(userRegisterVm);
            var user = await userManager.FindByNameAsync(userRegisterVm.Username);
            if (user != null)
            {
                ModelState.AddModelError("", "This username already taken");
                return View(userRegisterVm);
            }
            user = new AppUser
            {
                UserName = userRegisterVm.Username,
                Email = userRegisterVm.Email,
                FullName = userRegisterVm.FullName
            };
            var result = await userManager.CreateAsync(user, userRegisterVm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userRegisterVm);
            }

            await userManager.AddToRoleAsync(user, "Member");

            // create email message

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { email = user.Email, token }, Request.Scheme);





            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("mahabbatag@code.edu.az"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Confirm Email Address";
            using StreamReader reader = new StreamReader("wwwroot/templates/EmailConfirmTemplate.html");
            {
                string html = reader.ReadToEnd();
                html = html.Replace("{{link}}", confirmationLink);
                html = html.Replace("{{username}}", user.UserName);
                email.Body = new TextPart(TextFormat.Html) { Text = html };
            }

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("mahabbatag@code.edu.az", "lexk eiml satu syqx");
            smtp.Send(email);
            smtp.Disconnect(true);

            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            if (!await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", token))
            {
                return Content("Token is not valid");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            await userManager.UpdateSecurityStampAsync(user);
            if (!result.Succeeded)
            {
                return Content("Something went wrong");
            }
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> CreateRole()
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            }
            if (!await roleManager.RoleExistsAsync("Member"))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "Member" });
            }
            return Content("Created.");
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UserProfile(string tab = "dashboard")
        {
            ViewBag.Tab = tab;
            UserProfileVm userProfileVm = new UserProfileVm();
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            userProfileVm.UserUpdateProfileVm = new UserUpdateProfileVm
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email
            };
            return View(userProfileVm);
        }
        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UserProfile(UserUpdateProfileVm userUpdateProfileVm)
        {
            ViewBag.Tab = "profile";
            if (!ModelState.IsValid)
                return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account");
            var isExistUserName = await userManager.FindByNameAsync(userUpdateProfileVm.UserName);
            if (isExistUserName != null && isExistUserName.Id != user.Id)
            {
                ModelState.AddModelError("UserName", "This username already taken");
                return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
            }
            var isExistEmail = await userManager.FindByEmailAsync(userUpdateProfileVm.Email);
            if (isExistEmail != null && isExistEmail.Id != user.Id)
            {
                ModelState.AddModelError("Email", "This email already taken");
                return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
            }
            user.FullName = userUpdateProfileVm.FullName;
            user.UserName = userUpdateProfileVm.UserName;
            user.Email = userUpdateProfileVm.Email;
            if (!string.IsNullOrWhiteSpace(userUpdateProfileVm.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(userUpdateProfileVm.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required");
                    return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
                }
                var isCurrentPassword = await userManager.CheckPasswordAsync(user, userUpdateProfileVm.CurrentPassword);
                if (!isCurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                    return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
                }
                if (userUpdateProfileVm.NewPassword != userUpdateProfileVm.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                    return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
                }
                var isSamePassword = await userManager.CheckPasswordAsync(user, userUpdateProfileVm.NewPassword);
                if (isSamePassword)
                {
                    ModelState.AddModelError("NewPassword", "New password cannot be the same as the current password");
                    return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
                }

                var result = await userManager.ChangePasswordAsync(user, userUpdateProfileVm.CurrentPassword, userUpdateProfileVm.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
                }
            }
            var identityresult = await userManager.UpdateAsync(user);
            if (!identityresult.Succeeded)
            {
                foreach (var error in identityresult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(new UserProfileVm { UserUpdateProfileVm = userUpdateProfileVm });
            }
            await signInManager.SignInAsync(user, true);
            return RedirectToAction("UserProfile", "Account", new { tab = "profile" });
        }
    }
}

