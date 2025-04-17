using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Data;
using OnlineMobileStore.Models;

namespace OnlineMobileStore.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Company.ToListAsync());
        }
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.ProfileImageFile != null && company.ProfileImageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(company.ProfileImageFile.FileName)?.ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ProfileImageFile", "Please upload only images with the following extensions: .jpg, .jpeg, .png");
                        return View(company);
                    }

                    var imagePath = "/images/";
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + company.ProfileImageFile.FileName;

                    var webRootPath = _webHostEnvironment.WebRootPath;
                    var filePath = Path.Combine(webRootPath, "images", uniqueFileName);

                    var directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await company.ProfileImageFile.CopyToAsync(stream);
                    }

                    company.Image = Path.Combine(imagePath, uniqueFileName);
                }

                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }


		// GET: Companies/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCompany = await _context.Company.FindAsync(id);

                    if (company.ProfileImageFile != null && company.ProfileImageFile.Length > 0)
                    {
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(company.ProfileImageFile.FileName)?.ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("ProfileImageFile", "Please upload only images with the following extensions: .jpg, .jpeg, .png");
                            return View(company);
                        }

                        var imagePath = "/images/";
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + company.ProfileImageFile.FileName;

                        var webRootPath = _webHostEnvironment.WebRootPath;
                        var filePath = Path.Combine(webRootPath, "images", uniqueFileName);

                        var directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await company.ProfileImageFile.CopyToAsync(stream);
                        }

                        existingCompany.Image = Path.Combine(imagePath, uniqueFileName);
                    }

                    existingCompany.Name = company.Name;

                    _context.Update(existingCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }



		// GET: Companies/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company != null)
            {
                _context.Company.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
