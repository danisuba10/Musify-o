namespace Application.DataTransferObjects.Responses
{
    public class LibraryItemResponse
    {
        public required Guid Id { get; set; }
        public required Guid ItemId { get; set; }
        public required string ItemType { get; set; }
        public required string Name { get; set; }
        public string? ImageLocation { get; set; }
        public string? Subtitle { get; set; }
        public DateTime SavedAt { get; set; }
    }
}
