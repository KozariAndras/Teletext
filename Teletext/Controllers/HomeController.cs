using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Teletext.Areas.Identity.Data;
using Teletext.Helpers;
using Teletext.Models;
using Teletext.Services;

namespace Teletext.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ITeletextDataHandler _dataHandler;

        private DbPopulator _populator;

        public HomeController(ILogger<HomeController> logger, TeletextContext context)
        {

            _logger = logger;
            _dataHandler = new EFDataHandler(context);
            _populator = new(_dataHandler);
            _populator.CreateData();
        }

        public IActionResult Index()
        {
            return View(_dataHandler.GetAllTVChannels().Result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ApplyFilters(DateOnly date, string channel,TimeSpan timeFrom, TimeSpan timeTo, Genre genre)
        {
            ViewData.Add("Date", date.ToString());
            ViewData.Add("Channel", channel);
            ViewData.Add("TimeFrom", timeFrom);
            ViewData.Add("TimeTo", timeTo);
            ViewData.Add("Genre", genre);
            return View("Index", _dataHandler.GetAllTVChannels().Result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}