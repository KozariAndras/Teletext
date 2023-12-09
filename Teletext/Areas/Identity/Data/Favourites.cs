using Teletext.Models;

namespace Teletext.Areas.Identity.Data
{
    public class Favourites
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public long ProgramId { get; set; }

        public TeletextUser? User { get; set; }
        public TVProgram? Program { get; set; }
    }
}
