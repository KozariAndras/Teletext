using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teletext.Models
{
    public class TVChannel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public int Number { get; set; }

        public virtual ICollection<TVProgram> Programs { get; set; }


        public TVChannel(string name, int number)
        {
            Programs = new List<TVProgram>();
            Name = name;
            Number = number;
        }

        public TVChannel()
        {
            Programs = new List<TVProgram>();
        }
    }
}
