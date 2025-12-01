namespace LibraryAPI.Models;

public class Author
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
