using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Teletext.Areas.Identity.Data;
using Teletext.Helpers;
using Teletext.Models;
using Teletext.Models.Dto;
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

            await FilterByDate(ref channels, date);

            return View("Index", channels);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region Filter functions

        public async Task<TeletextDto> FilterAll(TeletextUser user, DateOnly date, string channelName, TimeSpan timeFrom, TimeSpan timeTo, Genre genre)
        {
            var dto = await _repo.Channels.GetDTOChannels(user);
            await FilterByChannelName(ref dto, channelName);
            await FilterByDate(ref dto, date);
            await FilterByGenre(ref dto, genre);
            await FilterByTime(ref dto, timeFrom, timeTo);

            return dto;
        }

        public Task FilterByChannelName(ref TeletextDto dto, string channelName)
        {
            if (channelName is null) return Task.CompletedTask;
            if (channelName == "Any") return Task.CompletedTask;
            if (string.IsNullOrEmpty(channelName)) return Task.CompletedTask;

            dto.Channels = dto.Channels.Where(c => c.Name == channelName).ToList();
            return Task.CompletedTask;
        }

        public Task FilterByDate(ref TeletextDto dto, DateOnly date)
        {
            if (dto is null) return Task.CompletedTask;

            dto.Channels = dto.Channels.Where(c => c.Programs.Where(p => p.AiringSchedules.Any(s => s.StartDate >= date))
                .Any(p => p.AiringSchedules.Any(s => s.StartDate >= date))).ToList();

            return Task.CompletedTask;
        }

        public Task FilterByGenre(ref TeletextDto dto, Genre genre)
        {
            if (dto is null) return Task.CompletedTask;
            if (genre == Genre.All) return Task.CompletedTask;

            dto.Channels = dto.Channels.Where(c => c.Programs.Where(p => p.Genre == genre)
                       .Any(p => p.Genre == genre)).ToList();
            return Task.CompletedTask;
        }

        public Task FilterByTime(ref TeletextDto dto, TimeSpan timeFrom, TimeSpan timeTo)
        {
            if (dto is null) return Task.CompletedTask;
            if (timeFrom >= timeTo) return Task.CompletedTask;

            dto.Channels = dto.Channels.Where(c => c.Programs.Where(p => p.AiringSchedules.Any(s => s.Time >= timeFrom && s.Time <= timeTo))
                              .Any(p => p.AiringSchedules.Any(s => s.Time >= timeFrom && s.Time <= timeTo))).ToList();

            return Task.CompletedTask;
        }

        #endregion
    }
}