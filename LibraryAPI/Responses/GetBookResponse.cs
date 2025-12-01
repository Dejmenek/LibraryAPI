using System.Text.Json.Serialization;

namespace LibraryAPI.Responses;

public record GetBookResponse(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("year")] int Year,
    [property: JsonPropertyName("author")] GetAuthorResponse Author
);
