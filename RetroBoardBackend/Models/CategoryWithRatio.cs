namespace RetroBoardBackend.Models
{
    public class CategoryWithRatio
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Ratio { get; set; }
    }
}
