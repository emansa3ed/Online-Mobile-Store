using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Data;
using OnlineMobileStore.Models;
namespace OnlineMobileStore.Controllers
{
	public class ProfileController : Controller
	{
		private readonly ApplicationDbContext _dbContext;


		public ProfileController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;

		}
		[Authorize(Roles ="User")]
		public IActionResult Index()
		{
			var username = HttpContext.Session.GetString("UserName");
			var user = _dbContext.User.FirstOrDefault(u => u.UserName == username);
			return View(user);
		}
        [Authorize(Roles = "Admin")]
        public IActionResult AIndex()
        {
            var username = HttpContext.Session.GetString("UserName");
            var user = _dbContext.User.FirstOrDefault(u => u.UserName == username);
            return View(user);
        }
        [Authorize(Roles = "User")]
		public IActionResult History()
		{
			var username = HttpContext.Session.GetString("UserName");
			var orders = _dbContext.Order.Where(u => u.UserId == username).ToList();
			return View(orders);
		}
		[Authorize(Roles = "User")]
		public IActionResult EditProfile()
		{
			var username = HttpContext.Session.GetString("UserName");
			var user = _dbContext.User.FirstOrDefault(u => u.UserName == username);
			return View(user);
		}
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult EditProfile(User user)
        {
            var username = HttpContext.Session.GetString("UserName");
            var existing = _dbContext.User.FirstOrDefault(u => u.UserName == username);
            ModelState.Remove("UserName");
            ModelState.Remove("Email");
            ModelState.Remove("PhoneNum");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                if (user.Password != null)
                {
                    user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
                    existing.Password = user.Password;
                }

                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;

                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
         
            return View(user);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _dbContext.Order
                .Where(ord => ord.Id == id)
                .Include(ord => ord.OrderItems)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AEditProfile()
        {
            var username = HttpContext.Session.GetString("UserName");
            var user = _dbContext.User.FirstOrDefault(u => u.UserName == username);
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AEditProfile(User user)
        {
            var username = HttpContext.Session.GetString("UserName");
            var existing = _dbContext.User.FirstOrDefault(u => u.UserName == username);
			ModelState.Remove("UserName");
            ModelState.Remove("Email");
            ModelState.Remove("PhoneNum");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                if (user.Password != null)
                {
                    user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
                    existing.Password = user.Password;
                }

                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;

                _dbContext.SaveChanges();
                return RedirectToAction("AIndex");
            }
 
            return View(user);
        }

    }
}
