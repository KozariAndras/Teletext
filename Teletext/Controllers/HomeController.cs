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
        public async Task<IActionResult> ApplyFilters(DateOnly date, string channelName,TimeSpan timeFrom, TimeSpan timeTo, Genre genre)
        {
            var aspnetUser = await _userManager.GetUserAsync(HttpContext.User);
            var channels = await _repo.Channels.GetDTOChannels(aspnetUser);

            await _repo.Channels.FilterByDate(ref channels, date);

            return View("Index", channels);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}