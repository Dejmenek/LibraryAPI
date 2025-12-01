using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI.Requests;

public record BookRequest(
    [property: Required, JsonPropertyName("title")] string Title,
    [property: Range(0, int.MaxValue), JsonPropertyName("year")] int Year,
    [property: JsonPropertyName("authorId")] int AuthorId
);
