using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pataatZOpdracht.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace pataatZOpdracht.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PataatZaakDbContext _context;
        private readonly IHostingEnvironment _hosting;

        public ProductsController(PataatZaakDbContext context, IHostingEnvironment hosting)
        {
            _context = context;
            _hosting = hosting;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole == "admin")
            {
                var pataatZaakDbContext = _context.Products.Include(p => p.Category);
                return View(await pataatZaakDbContext.ToListAsync());
            }
            else if (userRole == "customer")
            {
                List<string> categoriesNames = _context.Categories
                .Select(category => category.Name)
                .ToList();

                ViewData["categoriesNames"] = categoriesNames;

                var pataatZaakDbContext = _context.Products.Include(p => p.Category);
                return View("Menu",await pataatZaakDbContext.ToListAsync());
                
            }
            else
            {
                return View("~/Views/Shared/sessionEnded.cshtml");
            }
            
            
        }

        //public async Task<IActionResult> Menu()
        //{
            
        //}

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,CategoryId,Name,Description,Image,Price,Discount")] Product product)
        {
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if(file != null)
                {
                    string images = Path.Combine(_hosting.WebRootPath, "images");
                    fileName = file.FileName;
                    string fullPath = Path.Combine(images, fileName);   
                    file.CopyTo(new FileStream(fullPath, FileMode.Create));

                    product.Image = fileName;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile file, [Bind("Id,CategoryId,Name,Description,Image,Price,Discount")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }


            try
            {
                string fileName = null;

                if (file != null)
                {
                    string images = Path.Combine(_hosting.WebRootPath, "images");
                    fileName = file.FileName;
                    string fullPath = Path.Combine(images, fileName);

                    // Naam van de oude image
                    string currentImageName = _context.Products
                         .Where(p => p.Id == id)
                         .Select(p => p.Image)
                         .FirstOrDefault();

                    // Oude bestand verwijderen
                    if (!string.IsNullOrEmpty(currentImageName))
                    {
                        string currentImagePath = Path.Combine(images, currentImageName);
                        if (System.IO.File.Exists(currentImagePath))
                        {
                            System.IO.File.Delete(currentImagePath);
                        }
                    }

                    // Nieuwe bestand opslaan
                    file.CopyTo(new FileStream(fullPath, FileMode.Create));
                }
                else
                {
                    // Als er geen nieuwe afbeelding is geüpload, behoud de oude afbeeldingsnaam
                    string currentImageName = _context.Products
                         .Where(p => p.Id == id)
                         .Select(p => p.Image)
                         .FirstOrDefault();
                    fileName = currentImageName;
                }

                // Bijwerken van het product met de nieuwe afbeeldingsnaam (of oude afbeeldingsnaam als er geen nieuwe is)
                product.Image = fileName;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
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








        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'PataatZaakDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
