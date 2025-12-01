using LibraryAPI.Models;
using LibraryAPI.Requests;
using LibraryAPI.Responses;

namespace LibraryAPI.Helpers;

public static class Mapper
{
    public static IEnumerable<GetBookResponse> ToGetBookResponses(this IEnumerable<Book> books)
    {
        return books.Select(b => b.ToBookResponse());
    }

    public static IEnumerable<GetAuthorResponse> ToGetAuthorResponses(this IEnumerable<Author> authors)
    {
        return authors.Select(a => a.ToAuthorResponse());
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

    public static GetBookResponse ToBookResponse(this Book book)
    {
        return new GetBookResponse(
            book.Id, book.Title, book.Year,
            new GetAuthorResponse(book.Author.Id, book.Author.FirstName, book.Author.LastName));
    }

    public static Author ToAuthor(this AuthorRequest request)
    {
        return new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
    }

    public static GetAuthorResponse ToAuthorResponse(this Author author)
    {
        return new GetAuthorResponse(author.Id, author.FirstName, author.LastName);
    }
}
