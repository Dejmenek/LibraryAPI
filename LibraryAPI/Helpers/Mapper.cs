using LibraryAPI.Models;
using LibraryAPI.Requests;
using LibraryAPI.Responses;

namespace LibraryAPI.Helpers;

public static class Mapper
{
    public static IEnumerable<GetBooksResponse> ToBooksWithAuthorsResponse(this IEnumerable<Book> books)
    {
        return books.Select(b => new GetBooksResponse(
            b.Id, b.Title, b.Year,
            new GetAuthorResponse(b.Author.Id, b.Author.FirstName, b.Author.LastName)));
    }

    public static Book ToBook(this BookRequest request)
    {
        return new Book
        {
            AuthorId = request.AuthorId,
            Title = request.Title,
            Year = request.Year
        };
    }
}
