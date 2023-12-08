using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Teletext.Areas.Identity.Data;
using Teletext.Helpers;
using Teletext.Models;

namespace Teletext.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly TeletextContext _context;

        private DbPopulator _populator;

        public HomeController(ILogger<HomeController> logger, TeletextContext context)
        {

            _logger = logger;
            _context = context;
            _populator = new(_context);
            _populator.CreateData();
        }

        public IActionResult Index()
        {
            return View(_context.Channels.Include(c => c.Programs).ThenInclude(p => p.Scheduels).ToList());
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