using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teletext.Models;

public class AiringSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public long TVProgramId { get; set; }

    [ForeignKey("TVProgramId")]
    public TVProgram TVProgram { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    [EnumDataType(typeof(DayOfWeek))]
    public DayOfWeek Day { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan Time { get; set; }

    
    public AiringSchedule(DayOfWeek day, TimeSpan time, DateOnly date, TVProgram program)
    {
        TVProgram = program;
        Day = day;
        Time = time;
        StartDate = date;
    }
    
    public AiringSchedule()
    {
    }
    
}
