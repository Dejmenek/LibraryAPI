namespace LibraryAPI.Responses;

public record ErrorResponse(string Message, Dictionary<string, string[]>? Errors = null)
{
    public Dictionary<string, string[]> Errors { get; init; } = Errors ?? [];
}
