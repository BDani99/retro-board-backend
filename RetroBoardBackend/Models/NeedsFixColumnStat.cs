namespace RetroBoardBackend.Models
{
    public class NeedsFixColumnStat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Ratio { get; set; }
    }
}
