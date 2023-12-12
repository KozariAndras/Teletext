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

        #region Channel functions


        public async Task<IActionResult> ChannelMenu()
        {
            var channels = await _repo.Channels.GetAll();
            return View("ChannelMenu",channels);
        }


        public async Task<IActionResult> OpenAddChannel()
        {
            return View("AddChannel");
        }


        public async Task<IActionResult> AddChannel(string name, string description)
        {
            
            return RedirectToAction("ChannelMenu");
        }


        public async Task<IActionResult> OpenEditChannel(long id)
        {
            var channel = await _repo.Channels.GetById(id);
            return View("EditChannel",channel);
        }


        public async Task<IActionResult> EditChannel(string name, string description, long id)
        {
            
            return RedirectToAction("ChannelMenu");
        }
        #endregion


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
            return View("ScheduleMenu", schedules);
        }

        public async Task<IActionResult> OpenAddSchedule()
        {
            ViewBag.Programs = await _repo.Programs.GetAll();
            
            return View("AddSchedule");
        }
        
        public async Task<IActionResult> AddSchedule(long TVProgramId,DateOnly startDate, DayOfWeek day, TimeSpan time)
        {
            if (TVProgramId == 0) return View("AddSchedule");

            var program = await _repo.Programs.GetById(TVProgramId);
            if (program.Schedules is not null && program.Schedules.Count > 0) startDate = program.Schedules.FirstOrDefault()!.StartDate;
            var schedule = new AiringSchedule
            {
                StartDate = startDate,
                Day = day,
                Time = time,
                TVProgram = program,
                TVProgramId = program.Id
            };

            if (program.Schedules is null) program.Schedules = new List<AiringSchedule>();
            program.Schedules.Add(schedule);
            await _repo.Programs.Update(program);

            return RedirectToAction("ScheduleMenu");
        }
     
        public async Task<IActionResult> OpenEditSchedule(long id)
        {
            ViewBag.Programs = await _repo.Programs.GetAll();
            var schedule = await _repo.AiringSchedules.GetById(id);
            return View("EditSchedule",schedule);
        }

        public async Task<IActionResult> EditSchedule(long TVProgramId, DateOnly startDate, DayOfWeek day, TimeSpan time, long id)
        {
            ViewBag.Programs = await _repo.Programs.GetAll();
            var program = await _repo.Programs.GetById(TVProgramId);
            if (program.Schedules.First()!.StartDate != startDate)
            {
                foreach (var airingSchedule in program.Schedules)
                {
                    if (airingSchedule.Id == id)
                    { 
                        airingSchedule.Day = day;
                        airingSchedule.Time = time;
                    }
                    airingSchedule.StartDate = startDate;
                }
            }

            await _repo.Programs.Update(program);
            return RedirectToAction("ScheduleMenu");
        }

        public async Task<IActionResult> DeleteSchedule(long id)
        {
            
            await _repo.AiringSchedules.Delete(id);
            return RedirectToAction("ScheduleMenu");
        }

        public async Task<IActionResult> DetailsSchedule(long id)
        {
            var schedule = await _repo.AiringSchedules.GetById(id);
            return View("DetailsSchedule",schedule);
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
