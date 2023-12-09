using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<TeletextUser> _userManager;
        private readonly ITeletextRepository _repo;
        //private readonly ITeletextDataHandler _dataHandler;

        public HomeController(ILogger<HomeController> logger, UserManager<TeletextUser> userManager, ITeletextRepository repo)
        {
            _logger = logger;
            _userManager = userManager;
            _repo = repo;          
        }

        public async Task<IActionResult> Index()
        {
            var aspnetUser = await _userManager.GetUserAsync(HttpContext.User);
            var dto = await _repo.Channels.GetDTOChannels(aspnetUser);

            return View(dto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ApplyFilters(DateOnly date, string channelName,TimeSpan timeFrom, TimeSpan timeTo, Genre genre)
        {
            ViewData.Add("Date", date.ToString());
            ViewData.Add("Channel", channelName);
            ViewData.Add("TimeFrom", timeFrom);
            ViewData.Add("TimeTo", timeTo);
            ViewData.Add("Genre", genre);

            //var filterdChannels = FilterAll(date, channelName, timeFrom, timeTo, genre);
            var aspnetUser = _userManager.GetUserAsync(HttpContext.User).Result;
            var filterdChannels = _repo.Channels.GetAll();
            return View("Index", filterdChannels);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Test()
        {
            var aspnetUser = await _userManager.GetUserAsync(HttpContext.User);

            return Ok();
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> AddToFavourites([FromQuery]long programId)
        {
            var aspnetUser = await _userManager.GetUserAsync(HttpContext.User);

            var fav = new Favourites
            {
                UserId = aspnetUser.Id,
                ProgramId = programId
            };
            
            await _repo.Favourites.Add(fav);
            
            return RedirectToAction("Index");
        }
    }
}