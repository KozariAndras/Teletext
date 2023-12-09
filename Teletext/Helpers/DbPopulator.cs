﻿using Microsoft.EntityFrameworkCore;
using Teletext.Areas.Identity.Data;
using Teletext.Models;
using Teletext.Services;

namespace Teletext.Helpers
{
    public class DbPopulator
    {
        private readonly ITeletextRepository _repo;
        private List<TVChannel> _tVChannels;
        private List<TVProgram> _tVPrograms;

        public DbPopulator(ITeletextRepository repo)
        {
            _repo = repo;
            _tVChannels = _repo.Channels.GetAll().Result;
            _tVPrograms = _repo.Programs.GetAll().Result;
        }

        public async Task CreateData()
        {     
            if (_tVChannels is null || _tVChannels.Count() == 0)
            {
                _tVChannels = new List<TVChannel>();                
                CreateChannels();
                await _repo.Channels.AddRange(_tVChannels);
            }

            if (_tVPrograms is null || _tVPrograms.Count() == 0)
            {
                _tVPrograms = new List<TVProgram>();
                CreatePrograms();
                await _repo.Programs.AddRange(_tVPrograms);
            }
        }

        public void CreateChannels()
        {
            _tVChannels.Add(new TVChannel("HBO", 10));
            _tVChannels.Add(new TVChannel("CNN", 20));
            _tVChannels.Add(new TVChannel("Discovery", 30));
        }

        public void CreatePrograms()
        {
            var airDaysWeekdays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            var airTimesWeekdays = new List<TimeSpan> { new TimeSpan(21, 0, 0), new TimeSpan(21, 0, 0), new TimeSpan(21, 0, 0), new TimeSpan(21, 0, 0), new TimeSpan(21, 0, 0) };

            var airDaysEveryDay = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
            var airTimesEveryDay = new List<TimeSpan> { new TimeSpan(20, 0, 0), new TimeSpan(20, 0, 0), new TimeSpan(20, 0, 0), new TimeSpan(20, 0, 0), new TimeSpan(20, 0, 0), new TimeSpan(22, 0, 0), new TimeSpan(22, 0, 0) };

            var airDaysWeekdays2 = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday };
            var airTimesWeekdays2 = new List<TimeSpan> { new TimeSpan(17, 30, 0), new TimeSpan(19, 0, 0), new TimeSpan(22, 30, 0) };

            var airDaysWeekend = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var airTimesWeekend = new List<TimeSpan> { new TimeSpan(12, 0, 0), new TimeSpan(15, 0, 0) };

            _tVPrograms.Add(new TVProgram("Breaking Bad", 60, 16, Genre.Drama, _tVChannels[0], airDaysWeekdays, airTimesWeekdays, new DateOnly(2023, 11, 9)));
            _tVPrograms.Add(new TVProgram("The Big Bang Theory", 30, 6, Genre.Comedy, _tVChannels[0], airDaysWeekdays2, airTimesWeekdays2, new DateOnly(2023, 10, 25)));
            _tVPrograms.Add(new TVProgram("News", 30, 12, Genre.News, _tVChannels[1], airDaysEveryDay, airTimesEveryDay, new DateOnly(2023, 12, 6)));
            _tVPrograms.Add(new TVProgram("Gold Rush", 45, 8, Genre.Documentary, _tVChannels[2], airDaysWeekend, airTimesWeekend, new DateOnly(2023, 12, 6)));
        }

        public void CreateFavourites()
        {

        }
    }
}
