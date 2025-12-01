using LibraryAPI.Data;
using LibraryAPI.Helpers;
using LibraryAPI.Models;
using LibraryAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var books = app.MapGroup("/books");
var authors = app.MapGroup("/authors");

books.MapGet("/", async ([FromQuery] long? authorId, ApplicationDbContext context) =>
{
    var query = context.Books.Include(b => b.Author).AsNoTracking();

    if (authorId.HasValue)
    {
        query = query.Where(b => b.AuthorId == authorId);
    }

    var books = await query.ToListAsync();

    return Results.Ok(books.ToGetBookResponses());
});

books.MapGet("/{id}", async (long id, ApplicationDbContext context) =>
{
    var book = await context.Books.Include(b => b.Author).AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    if (book is null) return Results.NotFound();

    var bookResponse = book.ToBookResponse();

    return Results.Ok(bookResponse);
});

books.MapPost("/", async ([FromBody] BookRequest request, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return Results.BadRequest(new { Message = "Validation failed", Errors = errors });
    }

    var authorExists = await context.Authors.AnyAsync(a => a.Id == request.AuthorId);
    if (!authorExists)
    {
        return Results.BadRequest(new { Message = $"Author with Id {request.AuthorId} does not exist." });
    }

    var book = request.ToBook();

    context.Books.Add(book);
    await context.SaveChangesAsync();

    var savedBook = await context.Books
    .Include(b => b.Author)
    .AsNoTracking()
    .FirstOrDefaultAsync(b => b.Id == book.Id);

    var bookResponse = savedBook.ToBookResponse();

    return Results.Created($"/books/{bookResponse.Id}", bookResponse);
});

books.MapPut("/{id}", async (long id, [FromBody] BookRequest request, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
{
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return Results.BadRequest(new { Message = "Validation failed", Errors = errors });
    }

    var book = await context.Books.FindAsync(id);

    if (book is null) return Results.NotFound();

    var authorExists = await context.Authors.AnyAsync(a => a.Id == request.AuthorId);
    if (!authorExists)
    {
        return Results.BadRequest(new { Message = $"Author with Id {request.AuthorId} does not exist." });
    }

    book.Year = request.Year;
    book.Title = request.Title;
    book.AuthorId = request.AuthorId;

    await context.SaveChangesAsync();

    return Results.NoContent();
});

books.MapDelete("/{id}", async (long id, ApplicationDbContext context) =>
    {
    var book = await context.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

        context.Books.Remove(book);
        await context.SaveChangesAsync();

        return Results.NoContent();
});

authors.MapGet("/", async (ApplicationDbContext context) =>
{
    var authors = await context.Authors.AsNoTracking().ToListAsync();
    return Results.Ok(authors.ToGetAuthorResponses());
});

authors.MapGet("/{id}", async (long id, ApplicationDbContext context) =>
{
    var author = await context.Authors.FindAsync(id);

    if (author is null) return Results.NotFound();

    var authorResponse = author.ToAuthorResponse();

    return Results.Ok(authorResponse);
});

authors.MapPost("/", async ([FromBody] AuthorRequest request, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return Results.BadRequest(new { Message = "Validation failed", Errors = errors });
    }

    var author = request.ToAuthor();

    context.Authors.Add(author);
    await context.SaveChangesAsync();

    var authorResponse = author.ToAuthorResponse();

    return Results.Created($"/authors/{authorResponse.Id}", authorResponse);
});

authors.MapPut("/{id}", async (long id, [FromBody] AuthorRequest request, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return Results.BadRequest(new { Message = "Validation failed", Errors = errors });
    }

    var author = await context.Authors.FindAsync(id);

    if (author is null) return Results.NotFound();

    author.FirstName = request.FirstName;
    author.LastName = request.LastName;

    await context.SaveChangesAsync();

    return Results.NoContent();
});

authors.MapDelete("/{id}", async (long id, ApplicationDbContext context) =>
{
    var author = await context.Authors.FindAsync(id);
    if (author is null) return Results.NotFound();

    context.Authors.Remove(author);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();