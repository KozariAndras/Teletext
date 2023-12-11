using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml;
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
        _context.Set<TEntity>().Add(entity);
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

    public virtual async Task<List<TEntity>> GetAll()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task Update(TEntity entity)
    {
        var result = _context.Set<TEntity>().Where(x => x == entity).FirstOrDefault();
        if (result is not null)
        {
            result = entity;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateRange(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            var result = await _context.Set<TEntity>().FindAsync(entity);
            if (result is not null)
            {
                result = entity;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task Delete(long id)
    {
        if (id <= 0) return;

        var entity = await _context.Set<TEntity>().FindAsync(id);
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRange(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            _context.Set<TEntity>().Remove(entity);
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAll()
    {
        if (_context.Set<TEntity>().Count() == 0) return;

        _context.Set<TEntity>().RemoveRange(await _context.Set<TEntity>().ToListAsync());
        await _context.SaveChangesAsync();
    }
}


public interface ITVChannelRepository : IRepository<TVChannel>
{
    Task<TeletextDto> GetDTOChannels(TeletextUser user);
}


public class TVChannelRepository : EFRepository<TVChannel>, ITVChannelRepository
{
    private readonly TeletextContext _context;

    public TVChannelRepository(TeletextContext context) : base(context)
    {
        _context = context;
    }
    public async Task<TeletextDto> GetDTOChannels(TeletextUser user)
    {


        var data = await _context.Channels
            .Include(m => m.Programs)
            .Include(m => m.Programs).ThenInclude(m => m.Schedules)
            .Include(m => m.Programs).ThenInclude(m => m.Favourites)
            .ToListAsync();


        if (user is not null)
        {
            return new TeletextDto
            {
                User = user,
                Channels = data.Select(m => new ChannelDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Number = m.Number,
                    Programs = m.Programs.Select(m => new ProgramDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Duration = m.Duration,
                        AgeRating = m.AgeRating,
                        Genre = m.Genre,
                        IsFavourite = m.Favourites.Any(f => f.UserId == user.Id),
                        FavouriteId = m.Favourites.FirstOrDefault(f => f.UserId == user.Id)?.Id ?? -1,
                        AiringSchedules = m.Schedules.Select(m => new AiringScheduleDto
                        {
                            Id = m.Id,
                            StartDate = m.StartDate,
                            Day = m.Day,
                            Time = m.Time
                        }).ToList()
                    }).ToList()
                }).ToList()

            };
        }
        else
        {
            return new TeletextDto
            {
                Channels = data.Select(m => new ChannelDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Number = m.Number,
                    Programs = m.Programs.Select(m => new ProgramDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Duration = m.Duration,
                        AgeRating = m.AgeRating,
                        Genre = m.Genre,
                        AiringSchedules = m.Schedules.Select(m => new AiringScheduleDto
                        {
                            Id = m.Id,
                            StartDate = m.StartDate,
                            Day = m.Day,
                            Time = m.Time
                        }).ToList()
                    }).ToList()
                }).ToList()

            };
        }
    }
}


public interface ITVProgramRepository : IRepository<TVProgram>
{
    new public Task<List<TVProgram>> GetAll();
}

public class TVProgramRepository : EFRepository<TVProgram>, ITVProgramRepository
{
    private readonly TeletextContext _context;

    public TVProgramRepository(TeletextContext context) : base(context)
    {
        _context = context;
    }


    public override async Task<List<TVProgram>> GetAll()
    {
        return await _context.Programs
            .Include(p => p.Channel)
            .Include(p => p.Schedules).ToListAsync();
    }
}

public interface ITeletextRepository
{
    ITVChannelRepository Channels { get; }
    ITVProgramRepository Programs { get; }
    EFRepository<AiringSchedule> AiringSchedules { get; }
    EFRepository<Favourites> Favourites { get; }
}


public class TeletextRepository : ITeletextRepository
{
    private TeletextContext _context;
    private ITVChannelRepository? _channelRepo;
    private ITVProgramRepository? _programRepo;
    private EFRepository<AiringSchedule>? _scheduleRepo;
    private EFRepository<Favourites>? _favouriteRepo;

    public ITVChannelRepository Channels
    {
        get
        {
            if (_channelRepo is null)
            {
                _channelRepo = new TVChannelRepository(_context);
            }
            return _channelRepo;
        }
    }

    public ITVProgramRepository Programs
    {
        get
        {
            if (_programRepo is null)
            {
                _programRepo = new TVProgramRepository(_context);
            }
            return _programRepo;
        }
    }

    public EFRepository<AiringSchedule> AiringSchedules
    {
        get
        {
            if (_scheduleRepo is null)
            {
                _scheduleRepo = new EFRepository<AiringSchedule>(_context);
            }
            return _scheduleRepo;
        }
    }

    public EFRepository<Favourites> Favourites
    {
        get
        {
            if (_favouriteRepo is null)
            {
                _favouriteRepo = new EFRepository<Favourites>(_context);
            }
            return _favouriteRepo;
        }
    }

    private bool disposed = false;

    public TeletextRepository(TeletextContext context)
    {
        _context = context;
    }
}