using Teletext.Models;

namespace Teletext.Services
{
    public interface ITeletextDataHandler
    {
        // CRUD functions for TVChannel
        Task<bool> AddTVChannel(TVChannel channel);
        Task<bool> AddRangeTVChannel(List<TVChannel> channels);
        Task<TVChannel> GetTVChannelById(long id);
        Task<List<TVChannel>> GetAllTVChannels();
        Task<bool> UpdateTVChannel(TVChannel channel);
        Task<bool> UpdateRangeTVChannels(List<TVChannel> channels);
        Task<bool> DeleteTVChannel(long id);
        Task<bool> DeleteRangeTVChannels(List<TVChannel> channels);
        Task<bool> DeleteAllTVChannels();

        // CRUD functions for TVProgram
        Task<bool> AddTVProgram(TVProgram program);
        Task<bool> AddRangeTVProgram(List<TVProgram> programs);
        Task<TVProgram> GetTVProgramById(long id);
        Task<List<TVProgram>> GetAllTVPrograms();
        Task<bool> UpdateTVProgram(TVProgram program);
        Task<bool> UpdateRangeTVPrograms(List<TVProgram> programs);
        Task<bool> DeleteTVProgram(long id);
        Task<bool> DeleteRangeTVPrograms(List<TVProgram> programs);
        Task<bool> DeleteAllTVPrograms();

        // CRUD functions for AiringSchedule
        Task<bool> AddAiringSchedule(AiringSchedule schedule);
        Task<bool> AddRangeAiringSchedule(List<AiringSchedule> schedules);
        Task<AiringSchedule> GetAiringScheduleById(long id);
        Task<List<AiringSchedule>> GetAllAiringSchedules();
        Task<bool> UpdateAiringSchedule(AiringSchedule schedule);
        Task<bool> UpdateRangeAiringSchedules(List<AiringSchedule> schedules);
        Task<bool> DeleteAiringSchedule(long id);
        Task<bool> DeleteRangeAiringSchedules(List<AiringSchedule> schedules);
        Task<bool> DeleteAllAiringSchedules();
    }

}
