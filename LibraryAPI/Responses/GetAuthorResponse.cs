using System.Text.Json.Serialization;

namespace LibraryAPI.Responses;

public record GetAuthorResponse(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("last_name")] string LastName
);
