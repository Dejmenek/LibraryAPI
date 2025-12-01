using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI.Requests;

public record AuthorRequest(
  [property: Required, JsonPropertyName("first_name")] string FirstName,
  [property: Required, JsonPropertyName("last_name")] string LastName
);
