namespace Teletext.Models.Dto
{
    public class  TeletextXML
    {
        public List<TVChannelXML>? Channels { get; set; }

        public TeletextXML(List<ChannelDto> channels)
        {
            Channels = channels.Select(c => new TVChannelXML(c.Name, c.Number, c.Programs)).ToList();
        }

        public TeletextXML()
        {
        }
    }
    public class TVChannelXML
    {
        public string? Name { get; set; }
        public int Number { get; set; }
        public List<TVProgramXML>? Programs { get; set; }

        public TVChannelXML(string name, int number, List<ProgramDto> programs) 
        {
            Name = name;
            Number = number;
            Programs = programs.Select(p => new TVProgramXML
            {
                Name = p.Name,
                Duration = p.Duration,
                AgeRating = p.AgeRating,
                Genre = p.Genre,
                AiringSchedules = p.AiringSchedules.Select(s => new AiringScheduleXML
                {
                    StartDate = s.StartDate.ToString(),
                    Day = s.Day.ToString(),
                    Time = s.Time.ToString()    
                }).ToList()
            }).ToList();

        }

        public TVChannelXML()
        {
        }
    }

    public class TVProgramXML
    {
        public string? Name { get; set; }
        public int Duration { get; set; }
        public int AgeRating { get; set; }
        public Genre Genre { get; set; }
        public List<AiringScheduleXML>? AiringSchedules { get; set; }
    }

    public class AiringScheduleXML
    {
        public string? StartDate { get; set; }
        public string? Day { get; set; }
        public string? Time { get; set; }
    }

}
