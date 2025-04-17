using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Data;
using OnlineMobileStore.Models;
using System.Security.Claims;
using SessionSample;
namespace OnlineMobileStore.Controllers
{

    public class AuthenticationController : Controller
    {

        private readonly ApplicationDbContext _dbContext;
        
        public AuthenticationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {

            if (ModelState.IsValid)
            {
                if (Models.User.UserRegisteration(user, _dbContext))
                {
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("Role", "User");
                    HttpContext.Session.SetString("IsRegistered", "true");
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    ViewData["Message"] = "Registration failed. Please check your UserName or email ";
                    HttpContext.Session.Remove("IsRegistered");
                }
            }

            return View(user);
        }


        [HttpGet]
        public IActionResult Login()

        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            ModelState.Remove("Email");
            ModelState.Remove("PhoneNum");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Role");

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            ClaimsIdentity identity = null;
            var r = Models.User.UserLogin(user, _dbContext);
            var IsAuthenticate = false;
            if (r == "User")
            {
                identity = new ClaimsIdentity(new[]
                {
             new Claim(ClaimTypes.Role,"User")

         }, CookieAuthenticationDefaults.AuthenticationScheme);
                IsAuthenticate = true;
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Role", "User");
                HttpContext.Session.SetString("IsLoggedIn", "true");

            }
            else if (r == "Admin")
            {
                identity = new ClaimsIdentity(new[]
                {
             new Claim(ClaimTypes.Role,"Admin")

         }, CookieAuthenticationDefaults.AuthenticationScheme);
                IsAuthenticate = true;
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetString("IsLoggedIn", "true");

            }


            if (IsAuthenticate)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                if (r == "Admin")
                {
                    return RedirectToAction("Index", "Mobile");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewData["Error"] = "Login failed. Please check your username and password.";
                HttpContext.Session.Remove("IsLoggedIn");
                return base.View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

    }
}

