namespace Teletext.Models.Dto;

public class TeletextDto
{
    public TeletextUser User { get; set; }
    public List<ChannelDto> Channels { get; set; }
}

public class ChannelDto
{
    public List<ProgramDto> Programs { get; set; }
    public string Name { get; set; }

}

public class ProgramDto
{
    public List<AiringScheduleDto> AiringSchedules { get; set; }
    public string Name { get; set; }
    public bool IsFavourite { get; set; }
}

public class AiringScheduleDto
{ }