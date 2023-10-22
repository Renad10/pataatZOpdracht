using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pataatZOpdracht.Models;

namespace pataatZOpdracht.Controllers
{
    public class CartsController : Controller
    {
        private readonly PataatZaakDbContext _context;

        public CartsController(PataatZaakDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            string sessionUserId = HttpContext.Session.GetString("userid");
            if(string.IsNullOrEmpty(sessionUserId))
            {
                
                return View("~/Views/Shared/sessionEnded.cshtml") ;
            }
            else
            {
                //int userId = Int32.Parse(HttpContext.Session.GetString("userid"));
                int userId = Int32.Parse(sessionUserId);

                var pataatZaakDbContext = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Prod).Include(c => c.User);
                return View(await pataatZaakDbContext.ToListAsync());
            }
              
        }

        public async Task<IActionResult> AddToCart(int productId)
        {
            int userId = int.Parse(HttpContext.Session.GetString("userid"));
            Cart cart = new Cart();
            cart.ProdId = productId;
            cart.UserId = userId;
            cart.Quantity = 1;
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            var pataatZaakDbContext = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Prod).Include(c => c.User);
            return RedirectToAction(nameof(Index), await pataatZaakDbContext.ToListAsync());
        }

        public async void ChangeQuantity(int productId, int quanity)
        {
            int userId = int.Parse(HttpContext.Session.GetString("userid"));
            Cart cart = new Cart();
            cart.ProdId = productId;
            cart.UserId = userId;
            cart.Quantity = quanity;
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();        
        }



        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Prod)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProdId"] = new SelectList(_context.Products, "Id", "Id", cart.ProdId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProdId,Quantity")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProdId);
            return View(cart);
        }


        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Prod)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'PataatZaakDbContext.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
          return (_context.Carts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
