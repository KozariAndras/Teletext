using Humanizer.Localisation;
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


        #region TVPrgoram functions


        public async Task<IActionResult> TVProgramMenu()
        {
            var programs = await _repo.Programs.GetAll();          
            return View(programs);
        }

        public async Task<IActionResult> OpenAddTVProgram()
        {
            ViewBag.Channels = await _repo.Channels.GetAll();
            return View("AddTVProgram");
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

            if (!IsValidTVProgramInput(name, duration, ageRating, channelName, genre)) return View();

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
            return View("Index");
        }

        public async Task<IActionResult> EditTVProgram(string name, int duration, int ageRating, string channelName, Genre genre, long id)
        {
            var channels = await _repo.Channels.GetAll();
            ViewBag.Channels = channels;

            if (!IsValidTVProgramInput(name, duration, ageRating, channelName, genre)) return View();

            var selectedProgram = await _repo.Programs.GetById(id);
            var channel = channels.FirstOrDefault(c => c.Name == channelName);
            selectedProgram.Name = name;
            selectedProgram.Duration = duration;
            selectedProgram.AgeRating = ageRating;
            selectedProgram.Genre = genre;
            selectedProgram.Channel = channel;

            await _repo.Programs.Update(selectedProgram);
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTVProgram(long id)
        {
            await _repo.Programs.Delete(id);
            var programs = await _repo.Programs.GetAll();
            return View("TVProgramMenu",programs);
        }

        public async Task<IActionResult> DetailsTVProgram(long id)
        {
            var program = await _repo.Programs.GetById(id);
            return View("DetailsTVProgram", program);
        }


        #endregion


        #region Schedule functions

        public async Task<IActionResult> ScheduleMenu()
        {
            var schedules = await _repo.AiringSchedules.GetAll();
            return View(schedules);
        }

        

        #endregion

        private bool IsValidTVProgramInput(string name, int duration, int ageRating, string channelName, Genre genre)
        {
            if (String.IsNullOrEmpty(name)) return false;
            if (duration < 0) return false;
            if (ageRating < 0) return false;
            if (String.IsNullOrEmpty(channelName)) return false;
            if (genre == Genre.All) return false;

            return true;
        }
    }
}
