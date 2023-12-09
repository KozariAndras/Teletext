using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Teletext.Models
{
    public class Favourites
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? UserId { get; set; }
        public long ProgramId { get; set; }

        public TeletextUser? User { get; set; }
        public TVProgram? Program { get; set; }
    }
}
