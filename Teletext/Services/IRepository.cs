using Microsoft.EntityFrameworkCore;
using Teletext.Areas.Identity.Data;
using Teletext.Models;
using Teletext.Models.Dto;

namespace Teletext.Services;

public interface IRepository<TEntity>
{
    Task Add(TEntity entity);
    Task AddRange(IEnumerable<TEntity> entities);
    Task<TEntity> GetById(long id);
    Task<List<TEntity>> GetAll();
    Task Update(TEntity entity);
    Task UpdateRange(List<TEntity> entities);
    Task Delete(long id);
    Task DeleteRange(List<TEntity> entities);
    Task DeleteAll();
}

public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly TeletextContext _context;

    public EFRepository(TeletextContext context)
    {
        _context = context;
    }

    public async Task Add(TEntity entity)
    {
        _context.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().AddRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<TEntity> GetById(long id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public Task<List<TEntity>> GetAll()
    {
        return null;
    }

    public Task Update(TEntity entity)
    {
        return Task.CompletedTask;
    }

    public Task UpdateRange(List<TEntity> entities)
    {
        return Task.CompletedTask;
    }

    public Task Delete(long id)
    {
        return Task.CompletedTask;
    }

    public Task DeleteRange(List<TEntity> entities)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAll()
    {
        return Task.CompletedTask;
    }
}

public interface ITVProgramRepository : IRepository<TVProgram>
{
    Task<TeletextDto> GetChannels(TeletextUser user);
}

public class TVProgramRepository : EFRepository<TVProgram>, ITVProgramRepository
{
    private readonly TeletextContext _context;

    public TVProgramRepository(TeletextContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TeletextDto> GetChannels(TeletextUser user)
    {
        var data = await _context.Channels
            .Include(m => m.Programs)
            .Include(m => m.Programs).ThenInclude(m => m.Schedules)
            .Include(m => m.Programs).ThenInclude(m => m.Favourites)
            .ToListAsync();

        return new TeletextDto
        {
            User = user,
            Channels = data.Select(c => new ChannelDto
            {
                Name = c.Name,
                Programs = c.Programs.Select(p => new ProgramDto
                {
                    Name = p.Name,
                    IsFavourite = p.Favourites.Any(f => f.UserId == user.Id),
                    AiringSchedules = p.Schedules.Select(s => new AiringScheduleDto
                    {
                    }).ToList()
                }).ToList()
            }).ToList()
        };  
    }
}