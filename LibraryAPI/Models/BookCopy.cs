namespace LibraryAPI.Models;

public class BookCopy
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public Book Book { get; set; } = null!;
}
