using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teletext.Areas.Identity.Data;
using Teletext.Models;

namespace Teletext.Services
{
    public class EFDataHandler : ITeletextDataHandler
    {
        private readonly TeletextContext _context;


        #region TVChannel CRUD

        public Task<bool> AddTVChannel(TVChannel channel)
        {
            if (_context.Channels.Any(x => x.Id == channel.Id)) return Task.FromResult(false);

            if (_context.Channels.Any(x => x.Name == channel.Name)) return Task.FromResult(false);

            if (_context.Channels.Any(x => x.Number == channel.Number)) return Task.FromResult(false);

            _context.Channels.Add(channel);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> AddRangeTVChannel(List<TVChannel> channels)
        {
            if (channels.Count() == 0) return Task.FromResult(false);

            foreach (TVChannel channel in channels)
            {
                AddTVChannel(channel);
            }
            return Task.FromResult(true);
        }

        public Task<TVChannel> GetTVChannelById(long id)
        {
            if (!_context.Channels.Any(x => x.Id == id)) return Task.FromResult<TVChannel>(null);

            return Task.FromResult(_context.Channels.Include(c => c.Programs).ThenInclude(p => p.Schedules).First(x => x.Id == id));
        }

        public Task<List<TVChannel>> GetAllTVChannels()
        {
            if (_context.Channels.Count() == 0) return Task.FromResult<List<TVChannel>>(null);

            return Task.FromResult(_context.Channels.Include(c => c.Programs).ThenInclude(p => p.Schedules).ToList());
        }

        public Task<bool> UpdateTVChannel(TVChannel channel)
        {
            if (!_context.Channels.Any(x => x.Id == channel.Id)) return Task.FromResult(false);

            TVChannel temp = _context.Channels.First(x => x.Id == channel.Id);
            temp = channel;
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> UpdateRangeTVChannels(List<TVChannel> channels)
        {
            if (channels.Count() == 0) return Task.FromResult(false);

            foreach (TVChannel channel in channels)
            {
               UpdateTVChannel(channel);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteTVChannel(long id)
        {
            if (!_context.Channels.Any(x => x.Id == id)) return Task.FromResult(false);

            _context.Channels.Remove(_context.Channels.First(x => x.Id == id));
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRangeTVChannels(List<TVChannel> channels)
        {
            if (channels.Count() == 0) return Task.FromResult(false);

            foreach (TVChannel channel in channels)
            {
                DeleteTVChannel(channel.Id);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAllTVChannels()
        {
            if (_context.Channels.Count() == 0) return Task.FromResult(false);

            _context.Channels.ExecuteDelete();
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        #endregion


        #region TVProgram CRUD 

        public Task<bool> AddTVProgram(TVProgram program)
        {
            if (_context.Programs.Any(x => x.Id == program.Id)) return Task.FromResult(false);

            if (_context.Programs.Any(x => x.Name == program.Name)) return Task.FromResult(false);

            _context.Programs.Add(program);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> AddRangeTVProgram(List<TVProgram> programs)
        {
            if (programs.Count() == 0) return Task.FromResult(false);

            foreach (TVProgram program in programs)
            {
                AddTVProgram(program);
            }
            return Task.FromResult(true);
        }

        public Task<TVProgram> GetTVProgramById(long id)
        {
            if (!_context.Programs.Any(x => x.Id == id)) return Task.FromResult<TVProgram>(null);

            return Task.FromResult(_context.Programs.Include(p => p.Channel).Include(p => p.Schedules).First(x => x.Id == id));
        }

        public Task<List<TVProgram>> GetAllTVPrograms()
        {
            if (_context.Programs.Count() == 0) return Task.FromResult<List<TVProgram>>(null);

            return Task.FromResult(_context.Programs.Include(p => p.Channel).Include(p => p.Schedules).ToList());
        }

        public Task<bool> UpdateTVProgram(TVProgram program)
        {
            if (!_context.Programs.Any(x => x.Id == program.Id)) return Task.FromResult(false);

            TVProgram temp = _context.Programs.First(x => x.Id == program.Id);
            temp = program;
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> UpdateRangeTVPrograms(List<TVProgram> programs)
        {
            if (programs.Count() == 0) return Task.FromResult(false);

            foreach (TVProgram program in programs)
            {
                UpdateTVProgram(program);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteTVProgram(long id)
        {
            if (!_context.Programs.Any(x => x.Id == id)) return Task.FromResult(false);

            _context.Programs.Remove(_context.Programs.First(x => x.Id == id));
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRangeTVPrograms(List<TVProgram> programs)
        {
            if (programs.Count() == 0) return Task.FromResult(false);

            foreach (TVProgram program in programs)
            {
                DeleteTVProgram(program.Id);
            }
            return Task.FromResult(true);            
        }

        public Task<bool> DeleteAllTVPrograms()
        {
            if (_context.Programs.Count() == 0) return Task.FromResult(false);

            _context.Programs.ExecuteDelete();
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        #endregion


        #region AiringSchedule CRUD

        public Task<bool> AddAiringSchedule(AiringSchedule schedule)
        {
            if (_context.AiringSchedules.Any(x => x.Id == schedule.Id)) return Task.FromResult(false);

            _context.AiringSchedules.Add(schedule);
            _context.SaveChanges();
            return Task.FromResult(true);           
        }

        public Task<bool> AddRangeAiringSchedule(List<AiringSchedule> schedules)
        {
            if (schedules.Count() == 0) return Task.FromResult(false);

            foreach (AiringSchedule schedule in schedules)
            {
                AddAiringSchedule(schedule);
            }
            return Task.FromResult(true);
        }

        public Task<AiringSchedule> GetAiringScheduleById(long id)
        {
            if (!_context.AiringSchedules.Any(x => x.Id == id)) return Task.FromResult<AiringSchedule>(null);

            return Task.FromResult(_context.AiringSchedules.First(x => x.Id == id));
        }

        public Task<List<AiringSchedule>> GetAllAiringSchedules()
        {
            if (_context.AiringSchedules.Count() == 0) return Task.FromResult<List<AiringSchedule>>(null);

            return Task.FromResult(_context.AiringSchedules.ToList());
        }

        public Task<bool> UpdateAiringSchedule(AiringSchedule schedule)
        {
            if (!_context.AiringSchedules.Any(x => x.Id == schedule.Id)) return Task.FromResult(false);

            AiringSchedule temp = _context.AiringSchedules.First(x => x.Id == schedule.Id);
            temp = schedule;
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> UpdateRangeAiringSchedules(List<AiringSchedule> schedules)
        {
            if (schedules.Count() == 0) return Task.FromResult(false);

            foreach (AiringSchedule schedule in schedules)
            {
                UpdateAiringSchedule(schedule);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAiringSchedule(long id)
        {
            if (!_context.AiringSchedules.Any(x => x.Id == id)) return Task.FromResult(false);

            _context.AiringSchedules.Remove(_context.AiringSchedules.First(x => x.Id == id));
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRangeAiringSchedules(List<AiringSchedule> schedules)
        {
            if (schedules.Count() == 0) return Task.FromResult(false);

            foreach (AiringSchedule schedule in schedules)
            {
                DeleteAiringSchedule(schedule.Id);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAllAiringSchedules()
        {
            if (_context.AiringSchedules.Count() == 0) return Task.FromResult(false);

            _context.AiringSchedules.ExecuteDelete();
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        #endregion


        public EFDataHandler(TeletextContext context)
        {
            _context = context;
        }
    }
}
