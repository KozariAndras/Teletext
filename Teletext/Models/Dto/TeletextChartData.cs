namespace Teletext.Models.Dto
{
    public class TeletextChartData
    {
        public WeekData WeekdayData { get; set; } = new WeekData();
        public WeekData WeekendData { get; set; } = new WeekData();

    }

    public class WeekData
    {
        public Dictionary<string, int> ByTime { get; set; } = new();
        public Dictionary<string, int> ByCount { get; set; } = new();

    }
}
