using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Requests;

public record BookRequest
{
    [Required]
    public string Title { get; init; } = string.Empty;
    [Range(0, int.MaxValue)]
    public int Year { get; init; }
    public int AuthorId { get; init; }
}
