namespace RetroBoardBackend.Models
{
    public class PieChartStats
    {
        public List<CategoryWithRatio> WentWellColumnStats { get; set; }
            = new List<CategoryWithRatio>();
        public List<CategoryWithRatio> NeedsFixColumnStats { get; set; }
            = new List<CategoryWithRatio>();
    }
}
