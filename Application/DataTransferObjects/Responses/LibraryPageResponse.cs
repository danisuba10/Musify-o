namespace Application.DataTransferObjects.Responses
{
    public class LibraryPageResponse
    {
        public required List<LibraryItemResponse> Items { get; set; }
        public string? LastSavedAt { get; set; }
        public string? LastItemId { get; set; }
    }
}
