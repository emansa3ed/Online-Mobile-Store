using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineMobileStore.Data;
using OnlineMobileStore.Models;
using SessionSample;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace OnlineMobileStore.Controllers
{
	public class CartController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
	
		public CartController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
			
		}
        [Authorize(Roles="User")]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<Mobile>>("ShoppingCart") ?? new List<Mobile>();
            
            decimal totalPrice = cart.Sum(item => item.Quantity * item.Price);
            int totalItems = cart.Sum(item => item.Quantity);

            ViewBag.TotalPrice = totalPrice;
            ViewBag.TotalItems = totalItems;

            return View(cart);
        }


        [Authorize(Roles = "User")]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _dbContext.User.FirstOrDefault(u => u.UserName == username);
            var mobile = _dbContext.Mobile.FirstOrDefault(m => m.Id == id);
            if (mobile != null && mobile.Quantity >= quantity && quantity > 0)
            {

                var cart = HttpContext.Session.Get<List<Mobile>>("ShoppingCart") ?? new List<Mobile>();
                var cartItem = cart.FirstOrDefault(item => item.Id == id);

                if (cartItem == null)
                {

                    cart.Add(new Mobile
                    {
                        Id = mobile.Id,
                        Name = mobile.Name,
                        Quantity = quantity,
                        Price = mobile.Price,
                        Processor = mobile.Processor,
                        Screen = mobile.Screen,
                        Battery = mobile.Battery,
                        OS = mobile.OS,
                        Camera = mobile.Camera,
                        Image = mobile.Image,
                        CompanyId = mobile.CompanyId,
                        Company = mobile.Company
                    });
                }
                else
                {
                    cartItem.Quantity += quantity;
                }

                HttpContext.Session.Set("ShoppingCart", cart);
            }

            else if ((mobile != null && mobile.Quantity < quantity))
            {

                TempData["Message"] = $"Sorry, the available quantity of {mobile.Name} is only {mobile.Quantity}. Please reduce the quantity.";
            }

            return RedirectToAction("Index", "Cart");
        }


        [Authorize(Roles = "User")]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.Get<List<Mobile>>("ShoppingCart") ?? new List<Mobile>();

            var removedItem = cart.FirstOrDefault(item => item.Id == id);
            if (removedItem != null)
            {
                cart.Remove(removedItem);
                HttpContext.Session.Set("ShoppingCart", cart);
            }

            return RedirectToAction("Index", "Cart");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult UpdateCart(List<Mobile> cartUpdates)
        {
            var cart = HttpContext.Session.Get<List<Mobile>>("ShoppingCart") ?? new List<Mobile>();

            foreach (var update in cartUpdates)
            {
                var item = cart.FirstOrDefault(i => i.Id == update.Id);
                if (item != null)
                {
                    var mobile = _dbContext.Mobile.FirstOrDefault(m => m.Id == update.Id);
                    if (mobile != null)
                    {
                        if (update.Quantity <= mobile.Quantity && update.Quantity >= 0)
                        {
                            item.Quantity = update.Quantity;
                        }
                        else
                        {
                            TempData["Message"] = $"Sorry, the available quantity of {mobile.Name} is only {mobile.Quantity}. Please reduce the quantity.";
                        }
                    }
                }
            }
            HttpContext.Session.Set("ShoppingCart", cart);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "User")]
        public IActionResult ConfirmOrder()
        {
            var cart = HttpContext.Session.Get<List<Mobile>>("ShoppingCart") ?? new List<Mobile>();

            decimal totalPrice = cart.Sum(item => item.Quantity * item.Price);
            int totalItems = cart.Sum(item => item.Quantity);

            ViewBag.TotalPrice = totalPrice;
            ViewBag.TotalItems = totalItems;

            return View(cart);
        }




        [Authorize(Roles = "User")]
		public IActionResult Payment()
		{
			return View();
		}
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Payment(Payment payment)
        {

            ModelState.Remove("OrderId");
            ModelState.Remove("PaymentDate");
            ModelState.Remove("Amount");

            if (ModelState.IsValid)
            {
                var cart = HttpContext.Session.Get<List<Mobile>>("ShoppingCart") ?? new List<Mobile>();
                decimal amountOfOrder = 0;
                int totalItems = 0;

                foreach (var mobile in cart)
                {
                    amountOfOrder += mobile.Price * mobile.Quantity;
                    totalItems += mobile.Quantity;
                }

                var username = HttpContext.Session.GetString("UserName");
                var user = _dbContext.User.FirstOrDefault(u => u.UserName == username);

                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalPrice = amountOfOrder,
                    TotalItems = totalItems,
                    UserId = user.UserName,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var mobile in cart)
                {
                    var dbMobile = _dbContext.Mobile.FirstOrDefault(m => m.Id == mobile.Id);
                    if (dbMobile != null)
                    {
                        dbMobile.Quantity -= mobile.Quantity;

                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            MobileId = mobile.Id,
                            Quantity = mobile.Quantity,
                            Price = mobile.Price,
                            Name = mobile.Name,
                            Image = mobile.Image
                        };

                        order.OrderItems.Add(orderItem);
                    }

                    _dbContext.Order.Add(order);
                    _dbContext.SaveChanges();
                    int orderId = order.Id;
                    payment.OrderId = orderId;
                    payment.Amount = amountOfOrder;
                    payment.PaymentDate = DateTime.Now;
                    _dbContext.Payment.Add(payment);
                    var result = _dbContext.SaveChanges();

                    if (result <= 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    cart.Clear();
                    HttpContext.Session.Set("ShoppingCart", cart);
                    return RedirectToAction("Confirmation");
                }

            }

            return View(payment);
        }

        [Authorize(Roles = "User")]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}

