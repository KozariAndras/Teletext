using Microsoft.EntityFrameworkCore;
using Teletext.Areas.Identity.Data;
using Teletext.Models;
using Teletext.Services;

namespace Teletext.Helpers
{
    public class DbPopulator
    {
        private readonly ITeletextDataHandler _dataHandler;
        private List<TVChannel> _tVChannels;
        private List<TVProgram> _tVPrograms;

        public DbPopulator(ITeletextDataHandler dataHandler)
        {
            _dataHandler = dataHandler;
            _tVChannels = _dataHandler.GetAllTVChannels().Result;
            _tVPrograms = _dataHandler.GetAllTVPrograms().Result;
        }

        public void CreateData()
        {     
            if (_tVChannels is null)
            {
                _tVChannels = new List<TVChannel>();
                _tVPrograms = new List<TVProgram>();
                _dataHandler.DeleteAllTVPrograms();
                _dataHandler.DeleteAllAiringSchedules();
                CreateChannels();
                CreatePrograms();

                _dataHandler.AddRangeTVChannel(_tVChannels);
                _dataHandler.AddRangeTVProgram(_tVPrograms);          
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

            _tVPrograms.Add(new TVProgram("Breaking Bad", 60, 16, Genre.Drama, _tVChannels[0], airDaysWeekdays, airTimesWeekdays));
            _tVPrograms.Add(new TVProgram("The Big Bang Theory", 30, 6, Genre.Comedy, _tVChannels[0], airDaysWeekdays2, airTimesWeekdays2));
            _tVPrograms.Add(new TVProgram("News", 30, 12, Genre.News, _tVChannels[1], airDaysEveryDay, airTimesEveryDay));
            _tVPrograms.Add(new TVProgram("Gold Rush", 45, 8, Genre.Documentary, _tVChannels[2], airDaysWeekend, airTimesWeekend));
        }
    }
}
