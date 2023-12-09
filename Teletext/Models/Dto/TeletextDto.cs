namespace Teletext.Models.Dto;

public class TeletextDto
{
    public TeletextUser? User { get; set; }
    public List<ChannelDto>? Channels { get; set; }
}

public class ChannelDto
{
    public List<ProgramDto>? Programs { get; set; }

    public long Id { get; set; }

    public string? Name { get; set; }
    public int Number { get; set; }

}

public class ProgramDto
{
    public List<AiringScheduleDto>? AiringSchedules { get; set; }
    public long Id { get; set; }
    public string? Name { get; set; }
    public int Duration { get; set; }
    public int AgeRating { get; set; }
    public bool IsFavourite { get; set; }
}

public class AiringScheduleDto
{
    public long Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan Time { get; set; }

}