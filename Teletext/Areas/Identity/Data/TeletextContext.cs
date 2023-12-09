using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Teletext.Models;

namespace Teletext.Areas.Identity.Data;

public class TeletextContext : IdentityDbContext<TeletextUser>
{
    public DbSet<TVChannel> Channels { get; set; }
    public DbSet<TVProgram> Programs { get; set; }
    public DbSet<AiringSchedule> AiringSchedules { get; set; }
    public DbSet<Favourites> Favourites { get; set; }

    public TeletextContext(DbContextOptions<TeletextContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TVProgram>()
            .HasOne(p => p.Channel)
            .WithMany(c => c.Programs)
            .HasForeignKey(p => p.ChannelId);
    }
}
