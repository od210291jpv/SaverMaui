namespace SaverBackend.DTO
{
    public class StatisticsDto
    {
        public List<StatisticsItem> Items { get; set; }

        public StatisticsDto()
        {
            this.Items = new List<StatisticsItem>();
        }
    }
}
