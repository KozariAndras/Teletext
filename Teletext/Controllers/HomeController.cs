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
        private readonly TeletextContext _context;
        private readonly ITVProgramRepository _repo;
        private readonly ITeletextDataHandler _dataHandler;

        private DbPopulator _populator;

        public HomeController(
            ILogger<HomeController> logger, 
            TeletextContext context, 
            UserManager<TeletextUser> userManager, 
            ITVProgramRepository repo)
        {

            _logger = logger;
            _userManager = userManager;
            _context = context;
            _repo = repo;
            _dataHandler = new EFDataHandler(context);
            _populator = new(_dataHandler);
            //_populator.CreateData();

            
        }

        public async Task<IActionResult> Index()
        {
            var aspnetUser = await _userManager.GetUserAsync(HttpContext.User);
            var dto = await _repo.GetChannels(aspnetUser);

            // return View(dto);

            return View(_dataHandler.GetAllTVChannels().Result);
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

            var filterdChannels = FilterAll(date, channelName, timeFrom, timeTo, genre);
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


        private List<TVChannel> FilterAll(DateOnly date, string channelName, TimeSpan timeFrom, TimeSpan timeTo, Genre genre)
        {
            var channels = _dataHandler.GetAllTVChannels().Result;
            
            FilterByDate(ref channels, date);
            FilterByChannel(ref channels, channelName);
            FilterByTime(ref channels, timeFrom, timeTo);
            FilterByGenre(ref channels, genre);

            return channels;
        }

        private void FilterByDate(ref List<TVChannel> channels,DateOnly date)
        {
            channels = channels.Where(c => c.Programs.Any(p => p.Schedules.Any(s => s.StartDate >= date))).ToList();
        }

        private void FilterByChannel(ref List<TVChannel> channels, string channelName)
        {
            if (channelName is null) return;

            channels = channels.Where(c => c.Name == channelName).ToList();
        }

        private void FilterByTime(ref List<TVChannel> channels, TimeSpan timeFrom, TimeSpan timeTo)
        {
            if (timeFrom > timeTo) return;

            channels = channels.Where(c => c.Programs.Any(p => p.Schedules.Any(s => s.Time >= timeFrom && s.Time <= timeTo))).ToList();
        }

        private void FilterByGenre(ref List<TVChannel> channels, Genre genre)
        {
            if (genre == Genre.All) return;

            channels = channels.Where(c => c.Programs.Any(p => p.Genre == genre)).ToList();
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
            _context.Favourites.Add(fav);   
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}