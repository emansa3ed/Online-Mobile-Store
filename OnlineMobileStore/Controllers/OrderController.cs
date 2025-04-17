using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Data;
using OnlineMobileStore.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineMobileStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Order.Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Where(ord => ord.Id == id)
                .Include(ord => ord.OrderItems)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        public async Task<IActionResult> Order_Search(string search_word)
        {
            var order = _context.Order.Include(o => o.User).AsQueryable(); // Ensure related User data is loaded

            if (!string.IsNullOrEmpty(search_word))
            {
                // Try parsing search_word as order ID
                if (int.TryParse(search_word, out int searchId))
                {
                    order = order.Where(b => b.Id == searchId);
                }
                else
                {
                    // Search by user name
                    order = order.Where(b => b.User.UserName.Contains(search_word));

                    // Try parsing search_word as a date
                    if (DateTime.TryParse(search_word, out DateTime searchDate))
                    {
                        // Search for orders placed on the specified date
                        order = order.Where(b => b.OrderDate.HasValue && b.OrderDate.Value.Date == searchDate.Date);
                    }
                }
            }

            return View(await order.ToListAsync());
        }
        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}