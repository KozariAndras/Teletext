using Teletext.Models;

namespace Teletext.Services
{
    public interface ITeletextDataHandler
    {
        // CRUD functions for TVChannel
        Task<TVChannel> CreateTVChannel(TVChannel channel);
        Task<TVChannel> GetTVChannelById(long id);
        Task<List<TVChannel>> GetAllTVChannels();
        Task<TVChannel> UpdateTVChannel(TVChannel channel);
        Task<bool> DeleteTVChannel(long id);

        // CRUD functions for TVProgram
        Task<TVProgram> CreateTVProgram(TVProgram program);
        Task<TVProgram> GetTVProgramById(long id);
        Task<List<TVProgram>> GetAllTVPrograms();
        Task<TVProgram> UpdateTVProgram(TVProgram program);
        Task<bool> DeleteTVProgram(long id);

        // CRUD functions for AiringSchedule
        Task<AiringSchedule> CreateAiringSchedule(AiringSchedule schedule);
        Task<AiringSchedule> GetAiringScheduleById(long id);
        Task<List<AiringSchedule>> GetAllAiringSchedules();
        Task<AiringSchedule> UpdateAiringSchedule(AiringSchedule schedule);
        Task<bool> DeleteAiringSchedule(long id);
    }
}
