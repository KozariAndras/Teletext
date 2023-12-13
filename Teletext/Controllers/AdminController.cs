using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Teletext.Models;
using Teletext.Models.Dto;
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

            var chartData = await GetChartData();

            return View("Index",chartData);
        }

        #region Channel functions


        public async Task<IActionResult> ChannelMenu()
        {
            var channels = await _repo.Channels.GetAll();
            return View("ChannelMenu",channels);
        }


        public IActionResult OpenAddChannel()
        {
            return View("AddChannel");
        }


        public async Task<IActionResult> AddChannel(string name, int number)
        {
            if (!await IsValidNewTVChannelInput(name, number)) return View("AddChannel");

            var newChannel = new TVChannel
            {
                Name = name,
                Number = number,
                Programs = new List<TVProgram>()
            };
            await _repo.Channels.Add(newChannel);

            return RedirectToAction("ChannelMenu");
        }


        public async Task<IActionResult> OpenEditChannel(long id)
        {
            var channel = await _repo.Channels.GetById(id);
            return View("EditChannel",channel);
        }


        public async Task<IActionResult> EditChannel(string name, int number, long id)
        {
            if (!await IsValidTVChannelInput(name, number,id)) return View("EditChannel");

            var channel = await _repo.Channels.GetById(id);
            channel.Name = name;
            channel.Number = number;
            await _repo.Channels.Update(channel);
            
            return RedirectToAction("ChannelMenu");
        }


        public async Task<IActionResult> DeleteChannel(long id)
        {
            await _repo.Channels.Delete(id);
            return RedirectToAction("ChannelMenu");
        }


        public async Task<IActionResult> DetailsChannel(long id)
        {
            var channel = await _repo.Channels.GetById(id);
            return View("DetailsChannel",channel);
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

            if (!await IsValidNewTVProgramInput(name, duration, ageRating, channelName, genre)) return View();

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

            if (!await IsValidTVProgramInput(name, duration, ageRating, channelName, genre, id)) return View();

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
            return RedirectToAction("TVProgramMenu");
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


        #region Private functions

        private async Task<TeletextChartData> GetChartData()
        {
            var programs = await _repo.Programs.GetAll();

            TeletextChartData chartData = new();

            foreach (var program in programs)
            {
                foreach (var schedule in program.Schedules)
                {
                    if (schedule.Day == DayOfWeek.Saturday || schedule.Day == DayOfWeek.Sunday)
                    {
                        if (chartData.WeekendData.ByCount.ContainsKey(program.Genre.ToString()))
                        {
                            chartData.WeekendData.ByCount[program.Genre.ToString()]++;
                        }
                        else
                        {
                            chartData.WeekendData.ByCount.Add(program.Genre.ToString(), 1);
                        }

                        if (chartData.WeekendData.ByTime.ContainsKey(program.Genre.ToString()))
                        {
                            chartData.WeekendData.ByTime[program.Genre.ToString()] += program.Duration;
                        }
                        else
                        {
                            chartData.WeekendData.ByTime.Add(program.Genre.ToString(), program.Duration);
                        }
                    }
                    else
                    {
                        if (chartData.WeekdayData.ByCount.ContainsKey(program.Genre.ToString()))
                        {
                            chartData.WeekdayData.ByCount[program.Genre.ToString()]++;
                        }
                        else
                        {
                            chartData.WeekdayData.ByCount.Add(program.Genre.ToString(), 1);
                        }

                        if (chartData.WeekdayData.ByTime.ContainsKey(program.Genre.ToString()))
                        {
                            chartData.WeekdayData.ByTime[program.Genre.ToString()] += program.Duration;
                        }
                        else
                        {
                            chartData.WeekdayData.ByTime.Add(program.Genre.ToString(), program.Duration);
                        }
                    }
                }
            }

            return chartData;
        }

        private async Task<bool> IsValidTVProgramInput(string name, int duration, int ageRating, string channelName, Genre genre, long id)
        {
            if (String.IsNullOrEmpty(name)) return false;
            if (String.IsNullOrWhiteSpace(name)) return false;
            if (duration < 0) return false;
            if (ageRating < 0) return false;
            if (String.IsNullOrEmpty(channelName)) return false;
            if (genre == Genre.All) return false;

            var programs = await _repo.Programs.GetAll();
            if (programs.Count == 0) return true;

            foreach (var program in programs)
            {
                if (program.Name == name && program.Id != id) return false;
            }

            return true;
        }

        private async Task<bool> IsValidNewTVProgramInput(string name, int duration, int ageRating, string channelName, Genre genre)
        {

            if (String.IsNullOrEmpty(name)) return false;
            if (String.IsNullOrWhiteSpace(name)) return false;
            if (duration < 0) return false;
            if (ageRating < 0) return false;
            if (String.IsNullOrEmpty(channelName)) return false;
            if (genre == Genre.All) return false;

            var programs = await _repo.Programs.GetAll();

            foreach (var program in programs)
            {
                if (program.Name == name) return false;
            }

            return true;
        }


        private async Task<bool> IsValidTVChannelInput(string name, int number, long id)
        {
            if (String.IsNullOrEmpty(name)) return false;
            if (String.IsNullOrWhiteSpace(name)) return false;
            if (number < 0) return false;

            var channels = await _repo.Channels.GetAll();
            foreach (var channel in channels)
            {
                if (channel.Name == name && channel.Id != id) return false;
                if (channel.Number == number && channel.Id != id) return false;
            }

            return true;
        }

        private async Task<bool> IsValidNewTVChannelInput(string name, int number)
        {
            if (String.IsNullOrEmpty(name)) return false;
            if (String.IsNullOrWhiteSpace(name)) return false;
            if (number < 0) return false;

            var channels = await _repo.Channels.GetAll();
            foreach (var channel in channels)
            {
                if (channel.Name == name) return false;
                if (channel.Number == number) return false;
            }

            return true;
        }

        #endregion

    }
}
