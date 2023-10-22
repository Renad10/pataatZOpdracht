using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pataatZOpdracht.Models;
using System.Data;
using System.Diagnostics;

namespace pataatZOpdracht.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PataatZaakDbContext _context;


        public HomeController(ILogger<HomeController> logger, PataatZaakDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetString("userid");
            if (userRole == "admin")
            {
                
                return View();
            }
            else if (userRole == "customer")
            {

                var pataatZaakDbContext = _context.Products.Include(p => p.Category).Where(p => p.Discount !=null);
                var userName = _context.Users.SingleOrDefault(u => u.Id == int.Parse(userId)).Name.ToString();
                ViewData["userName"] = userName;
                return View("homeCustomer", await pataatZaakDbContext.ToListAsync());

            }
            else
            {
                return View("~/Views/Shared/sessionEnded.cshtml");
            }


        }


        public IActionResult Privacy()
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