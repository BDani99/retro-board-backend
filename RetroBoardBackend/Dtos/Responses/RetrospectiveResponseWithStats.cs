using RetroBoardBackend.Models;

namespace RetroBoardBackend.Dtos.Responses
{
    public class RetrospectiveResponseWithStats
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool StatsNeeded { get; set; }
        public bool IsActive { get; set; }
        public EntryAmount EntryAmount { get; set; } = null!;

        public PieChartStats? PieChartStats { get; set; }
        public LineChartStats? LineChartStats { get; set; }
    }
}
