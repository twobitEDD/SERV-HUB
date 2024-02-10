namespace Architecture.Statistics.Interfaces
{
    // Component for statistic element
    public interface IStatistic
    {
        float StatisticsViewWidth { get; } // Width of statistics page
        ViewsLine ViewLine(); // Component with pages
    }
}
