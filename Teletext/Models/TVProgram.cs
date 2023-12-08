using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Teletext.Areas.Identity.Data;

namespace Teletext.Models
{
    public enum Genre
    {
        News,
        Culinary,
        Movie,
        TalkShow,
        Documentary,
        Sports,
        Comedy,
        Drama,
        RealityTV,
        GameShow
    }

    public class TVProgram
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public int AgeRating { get; set; }

        [Required]
        public long ChannelId { get; set; }

        [ForeignKey("ChannelId")]
        public virtual TVChannel Channel { get; set; }

        [Required]
        public Genre Genre { get; set; }

        public virtual List<AiringScheduel> Scheduels { get; set; }

        public virtual List<TeletextUser> FavBy { get; set; }


        public TVProgram(string name, int duration, int ageRating, Genre programGenre,TVChannel channel, List<DayOfWeek> airDays, List<TimeSpan> airTimes)
        {
            Scheduels = new();
            Name = name;
            AgeRating = ageRating;
            Duration = duration;
            Genre = programGenre;
            Channel = channel;

            if (airDays.Count != airTimes.Count) throw new Exception("Airing schedual missmatch!");

            for (int i = 0; i < airDays.Count; i++)
            {
                Scheduels.Add(new AiringScheduel(airDays[i], airTimes[i],this));
            }

        }

        public TVProgram()
        {
            Scheduels = new();
        }

    }

}
