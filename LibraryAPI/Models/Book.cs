namespace LibraryAPI.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}
