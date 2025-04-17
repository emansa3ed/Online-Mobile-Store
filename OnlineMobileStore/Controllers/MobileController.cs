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
using OnlineMobileStore.Services;
using OnlineMobileStore.ViewModels;


namespace OnlineMobileStore.Controllers
{
    public class MobileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMobileServices _mobileServices;

        public MobileController(ApplicationDbContext context, IMobileServices mobileServices)
        {
            _mobileServices = mobileServices;
            _context = context;
        }
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Mobile.Include(m => m.Company);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var companies = _context.Company.ToList();
            CreateMobileViewModel viewModel = new()
            {
                Companies = _context.Company.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .OrderBy(c => c.Text)
                .ToList()
            };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateMobileViewModel model)
        {
          if (!ModelState.IsValid)
            {
                model.Companies = _context.Company.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .OrderBy(c => c.Text)
                .ToList();
                return View(model);
            }
            // save mobile in db 
            // save image in server 
            await _mobileServices.Create(model);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var mobile = _context.Mobile.FirstOrDefault(m => m.Id == id);

            if (mobile == null)
            {
                return NotFound();
            }

            var viewModel = new CreateMobileViewModel
            {
                Id = mobile.Id,
                Name = mobile.Name,
                CompanyId = mobile.CompanyId,
                Quantity = mobile.Quantity,
                Price = mobile.Price,
                Processor = mobile.Processor,
                Screen = mobile.Screen,
                Battery = mobile.Battery,
                OS = mobile.OS,
                Camera = mobile.Camera,
                Companies = _context.Company.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                                   .OrderBy(c => c.Text)
                                   .ToList()
            };
            return View(viewModel);
         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(CreateMobileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Companies = _context.Company.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .OrderBy(c => c.Text)
                .ToList();
                return View(model);
            }
            // edit mobile in db 
            // edit image in server 
            await _mobileServices.Edit(model);
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mobile = _context.Mobile.Include(b => b.Company)
                .FirstOrDefault(b => b.Id == id);

            //var mobile = await _context.Mobile.Include(m => m.Company).FirstOrDefaultAsync(m => m.Id == id);
            if (mobile == null)
            {
                return NotFound();
            }

            return View(mobile);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mobile = await _context.Mobile
                .Include(m => m.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mobile == null)
            {
                return NotFound();
            }

            return View(mobile);
        }

        // POST: Mobiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mobile = await _context.Mobile.FindAsync(id);
            if (mobile != null)
            {
                _context.Mobile.Remove(mobile);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}