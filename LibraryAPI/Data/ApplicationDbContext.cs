using LibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);

        modelBuilder.Entity<Book>()
            .ToTable(b => b.HasCheckConstraint("CK_Book_Year", "Year >= 0"));

        modelBuilder.Entity<BookCopy>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.BookCopies)
            .HasForeignKey(bc => bc.BookId);
    }
}
