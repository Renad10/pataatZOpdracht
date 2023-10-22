using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pataatZOpdracht.Models;

namespace pataatZOpdracht.Controllers
{
    public class OrdersController : Controller
    {
        private readonly PataatZaakDbContext _context;

        public OrdersController(PataatZaakDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            string sessionUserId = HttpContext.Session.GetString("userid");
            if (string.IsNullOrEmpty(sessionUserId))
            {

                return View("~/Views/Shared/sessionEnded.cshtml");
            }
            else
            {
                int userId = Int32.Parse(sessionUserId);
                var pataatZaakDbContext = _context.Orders.Where(o => o.UserId == userId).Include(o => o.OrderItems);
                return View(await pataatZaakDbContext.ToListAsync());
            }
            
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {

            Order order = new Order();
            List<OrderItem> orderItems = new List<OrderItem>();
            int userId = int.Parse(HttpContext.Session.GetString("userid"));
            
            DateTime date = DateTime.Now;
            order.UserId = userId;
            order.CreatedAt = date;
            List<Cart> carts = _context.Carts.Where(oi => oi.UserId == userId).Include(c => c.Prod).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            
            foreach (Cart cart in carts)
            {
                OrderItem orderItem = new OrderItem();
                orderItem.Name = cart.Prod.Name;
                orderItem.Quantity = cart.Quantity;
                orderItem.OrderId = order.Id;
                if (cart.Prod.Discount != null)
                {
                    decimal priceAfterDiscount = cart.Prod.DiscountCalculate();
                    orderItem.Price= priceAfterDiscount;
                }
                else
                {
                    orderItem.Price = cart.Prod.Price;
                }

                orderItems.Add(orderItem);
                _context.Carts.Remove(cart);
            }
            foreach(OrderItem orderItem in orderItems)
            {
                _context.OrderItems.Add(orderItem);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CreatedAt")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'PataatZaakDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
