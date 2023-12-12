using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
            ViewBag.Channels = await _repo.Channels.GetAll();
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
            ViewBag.Channels = await _repo.Channels.GetAll();
            var aspnetUser = await _userManager.GetUserAsync(HttpContext.User);
            var channels = await _repo.Channels.GetDTOChannels(aspnetUser);

            var dto = await FilterAll(aspnetUser, date, channelName, timeFrom, timeTo, genre);

            return View("Index", dto);
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
            if (dto is null) return Task.CompletedTask;
            if (dto.Channels is null) return Task.CompletedTask;
            if (channelName is null) return Task.CompletedTask;
            if (channelName == "Any") return Task.CompletedTask;
            if (string.IsNullOrEmpty(channelName)) return Task.CompletedTask;

            for (int i = dto.Channels.Count - 1; i >= 0; i--)
            {
                if (dto.Channels[i].Name != channelName)
                {
                    dto.Channels.RemoveAt(i);
                }
            }
            return Task.CompletedTask;
        }

        public Task FilterByDate(ref TeletextDto dto, DateOnly date)
        {
            if (dto is null) return Task.CompletedTask;
            if (dto.Channels is null) return Task.CompletedTask;         
            if (date == DateOnly.MinValue) return Task.CompletedTask;

            for (int i = dto.Channels.Count -1; i >= 0; i--)
            {
                if (dto.Channels[i].Programs is null) continue;

                for (int j = dto.Channels[i].Programs.Count -1; j >= 0; j--)
                {
                    if (dto.Channels[i].Programs[j].AiringSchedules is null) continue;

                    for (int k = dto.Channels[i].Programs[j].AiringSchedules.Count -1; k >= 0; k--)
                    {
                        if (dto.Channels[i].Programs[j].AiringSchedules[k].StartDate >= date)
                        {
                            dto.Channels[i].Programs[j].AiringSchedules.RemoveAt(k);
                        }
                    }
                    if (dto.Channels[i].Programs[j].AiringSchedules.Count == 0)
                    {
                        dto.Channels[i].Programs.RemoveAt(j);
                    }
                }
                if (dto.Channels[i].Programs.Count == 0)
                {
                    dto.Channels.RemoveAt(i);
                }
            }

            return Task.CompletedTask;
        }

        public Task FilterByGenre(ref TeletextDto dto, Genre genre)
        {
            if (dto is null) return Task.CompletedTask;
            if (genre == Genre.All) return Task.CompletedTask;
            if (dto.Channels is null) return Task.CompletedTask;

            for (int i = dto.Channels.Count - 1; i >= 0; i--)
            {
                if (dto.Channels[i].Programs is null) continue;

                for (int j = dto.Channels[i].Programs.Count -1; j >= 0; j--)
                {
                    if (dto.Channels[i].Programs[j].Genre != genre)
                    {
                        dto.Channels[i].Programs.RemoveAt(j);
                    }
                }
                if (dto.Channels[i].Programs.Count == 0)
                {
                    dto.Channels.RemoveAt(i);
                }
            }

            return Task.CompletedTask;
        }

        public Task FilterByTime(ref TeletextDto dto, TimeSpan timeFrom, TimeSpan timeTo)
        {
            if (dto is null) return Task.CompletedTask;
            if (dto.Channels is null) return Task.CompletedTask;

            for (int i = dto.Channels.Count - 1; i >= 0; i--)
            {
                if (dto.Channels[i].Programs is null) continue;

                for (int j = dto.Channels[i].Programs.Count -1; j >= 0; j--)
                {
                    if (dto.Channels[i].Programs[j].AiringSchedules is null) continue;

                    for (int k = dto.Channels[i].Programs[j].AiringSchedules.Count -1; k >= 0; k--)
                    {
                        if (timeFrom < timeTo)
                        {
                            if (dto.Channels[i].Programs[j].AiringSchedules[k].Time <= timeFrom || dto.Channels[i].Programs[j].AiringSchedules[k].Time >= timeTo)
                            {
                                dto.Channels[i].Programs[j].AiringSchedules.RemoveAt(k);
                            }
                        }
                        else if ( timeFrom > timeTo)
                        {
                            if ((dto.Channels[i].Programs[j].AiringSchedules[k].Time <= timeFrom && dto.Channels[i].Programs[j].AiringSchedules[k].Time >= timeTo))
                            {
                                dto.Channels[i].Programs[j].AiringSchedules.RemoveAt(k);
                            }
                        }
                        else
                        {
                            if (dto.Channels[i].Programs[j].AiringSchedules[k].Time != timeFrom)
                            {
                                dto.Channels[i].Programs[j].AiringSchedules.RemoveAt(k);
                            }
                        }   
                        
                    }
                    if (dto.Channels[i].Programs[j].AiringSchedules.Count == 0)
                    {
                        dto.Channels[i].Programs.RemoveAt(j);
                    }

                }
                if (dto.Channels[i].Programs.Count == 0)
                {
                    dto.Channels.RemoveAt(i);
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}