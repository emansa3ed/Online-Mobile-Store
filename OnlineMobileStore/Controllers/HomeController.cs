using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Data;
using OnlineMobileStore.Migrations;
using OnlineMobileStore.Models;
using System.Diagnostics;

namespace OnlineMobileStore.Controllers
{
	//[Authorize]
	public class HomeController : Controller
	{

		private readonly ApplicationDbContext db;

		public HomeController(ApplicationDbContext context)
		{
			db = context;
		}

		public IActionResult Index()
		{

			var Company = db.Company.ToList();

			return View(Company);
		}


		public IActionResult Mobile(int id)
		{
			var result = db.Mobile.Where(x => x.Company.Id == id).ToList();
			return View(result);
		}
		public IActionResult ProductDetail(int id)
		{
			var result = db.Mobile.Where(x => x.Id == id).ToList();
			return View(result);
		}

        public async Task<IActionResult> Mobile_Search(string search_word)
        {
            var mobiles = db.Mobile.Select(b => b);

            if (!string.IsNullOrEmpty(search_word))
            {
                mobiles = mobiles.Where(b =>
                    b.Name.Contains(search_word)
                );
            }
            return View(await mobiles.ToListAsync());
        }


        public IActionResult AccessDenied()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
