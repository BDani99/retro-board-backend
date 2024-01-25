namespace RetroBoardBackend.Dtos
{
    public class EntryDto
    {
        public string EntryContent { get; set; } = string.Empty;
        public ICollection<int> CategoryIds { get; set; } = new List<int>();
        public int RetrospectiveId { get; set; }
        public int AssigneeId { get; set; }
        public string ColumnType { get; set; } = string.Empty;
    }
}
