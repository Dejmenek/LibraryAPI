namespace LibraryAPI.Responses;

public record GetBooksResponse(
    int Id,
    string Title,
    int Year,
    GetAuthorResponse Author
);
