using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Teletext.Models;
using Teletext.Services;

namespace Teletext.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<TeletextUser> _userManager;
        private readonly ITeletextRepository _repo;

        public AdminController(ILogger<HomeController> logger, UserManager<TeletextUser> userManager, ITeletextRepository repo)
        {
            _logger = logger;
            _userManager = userManager;
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Channels = await _repo.Channels.GetAll();
            return View();
        }

        public async Task<IActionResult> TVProgramMenu()
        {
            var programs = await _repo.Programs.GetAll();          
            return View(programs);
        }

        public async Task<IActionResult> OpenEditTVProgram(long id)
        {
            ViewBag.Channels = await _repo.Channels.GetAll();
            var program = await _repo.Programs.GetById(id);
            return View("EditTVProgram", program);
        }

        public async Task<IActionResult> AddTVProgram(string name, int duration, int ageRating, string channelName, Genre genre)
        {
            var channels = await _repo.Channels.GetAll();
            ViewBag.Channels = channels;

            if (String.IsNullOrEmpty(name)) return View();
            if (duration < 0) return View();
            if (ageRating < 0) return View();
            if (String.IsNullOrEmpty(channelName)) return View();
            if (genre == Genre.All) return View();

            var selectedChannel = channels.FirstOrDefault(c => c.Name == channelName);
            var program = new TVProgram
            {
                Name = name,
                Duration = duration,
                AgeRating = ageRating,
                Channel = selectedChannel,
                ChannelId = selectedChannel.Id,
                Genre = genre
            };
              
            await _repo.Programs.Add(program);
            return Redirect("Index");
        }

        public async Task<IActionResult> EditTVProgram(string name, int duration, int ageRating, string channelName, Genre genre, long id)
        {
            var channels = await _repo.Channels.GetAll();
            ViewBag.Channels = channels;

            if (String.IsNullOrEmpty(name)) return View();
            if (duration < 0) return View();
            if (ageRating < 0) return View();
            if (String.IsNullOrEmpty(channelName)) return View();
            if (genre == Genre.All) return View();

            var selectedProgram = await _repo.Programs.GetById(id);
            selectedProgram.Name = name;
            selectedProgram.Duration = duration;
            selectedProgram.AgeRating = ageRating;
            selectedProgram.Genre = genre;

            await _repo.Programs.Update(selectedProgram);
            return Redirect("Index");
        }

    }
}
