using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teletext.Models;

public class AiringScheduel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public long TVProgramId { get; set; }

    [ForeignKey("TVProgramId")]
    public TVProgram TVProgram { get; set; }

    [Required]
    [EnumDataType(typeof(DayOfWeek))]
    public DayOfWeek Day { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan Time { get; set; }

    
    public AiringScheduel(DayOfWeek day, TimeSpan time, TVProgram program)
    {
        TVProgram = program;
        Day = day;
        Time = time;
    }
    
    public AiringScheduel()
    {
    }
    
}
