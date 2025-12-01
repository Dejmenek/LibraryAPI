namespace LibraryAPI.Models;

public class Book
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public long AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}
