using LibraryAPI.Data;
using LibraryAPI.Helpers;
using LibraryAPI.Requests;
using LibraryAPI.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
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

books.MapGet("/", async Task<Ok<IEnumerable<GetBookResponse>>> (
    [FromQuery] long? authorId, CancellationToken token, ApplicationDbContext context) =>
{
    var query = context.Books.Include(b => b.Author).AsNoTracking();

    if (authorId.HasValue)
    {
        query = query.Where(b => b.AuthorId == authorId);
    }

    var books = await query.ToListAsync(token);

    return TypedResults.Ok(books.ToGetBookResponses());
});

books.MapGet("/{id}", async Task<Results<Ok<GetBookResponse>, NotFound>> (
    long id, CancellationToken token, ApplicationDbContext context) =>
{
    var book = await context.Books.Include(b => b.Author).AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, token);

    if (book is null) return TypedResults.NotFound();

    var bookResponse = book.ToBookResponse();

    return TypedResults.Ok(bookResponse);
});

books.MapPost("/", async Task<Results<BadRequest<ErrorResponse>, Created<GetBookResponse>>> (
    [FromBody] BookRequest request, CancellationToken token, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return TypedResults.BadRequest(new ErrorResponse("Validation failed", errors));
    }

    var authorExists = await context.Authors.AnyAsync(a => a.Id == request.AuthorId, token);
    if (!authorExists)
    {
        return TypedResults.BadRequest(new ErrorResponse($"Author with Id {request.AuthorId} does not exist."));
    }

    var book = request.ToBook();

    context.Books.Add(book);
    await context.SaveChangesAsync(token);

    var savedBook = await context.Books
    .Include(b => b.Author)
    .AsNoTracking()
    .FirstOrDefaultAsync(b => b.Id == book.Id, token);

    var bookResponse = savedBook.ToBookResponse();

    return TypedResults.Created($"/books/{bookResponse.Id}", bookResponse);
});

books.MapPut("/{id}", async Task<Results<BadRequest<ErrorResponse>, NoContent, NotFound>> (
    long id, [FromBody] BookRequest request, CancellationToken token, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return TypedResults.BadRequest(new ErrorResponse("Validation failed", errors));
    }

    var book = await context.Books.FindAsync([id], token);

    if (book is null) return TypedResults.NotFound();

    var authorExists = await context.Authors.AnyAsync(a => a.Id == request.AuthorId, token);
    if (!authorExists)
    {
        return TypedResults.BadRequest(new ErrorResponse($"Author with Id {request.AuthorId} does not exist."));
    }

    book.Year = request.Year;
    book.Title = request.Title;
    book.AuthorId = request.AuthorId;

    await context.SaveChangesAsync(token);

    return TypedResults.NoContent();
});

books.MapDelete("/{id}", async Task<Results<NotFound, NoContent>> (long id, CancellationToken token, ApplicationDbContext context) =>
{
    var book = await context.Books.FindAsync([id], token);
    if (book is null) return TypedResults.NotFound();

    context.Books.Remove(book);
    await context.SaveChangesAsync(token);

    return TypedResults.NoContent();
});

authors.MapGet("/", async Task<Ok<IEnumerable<GetAuthorResponse>>> (
    CancellationToken token, ApplicationDbContext context) =>
{
    var authors = await context.Authors.AsNoTracking().ToListAsync(token);
    return TypedResults.Ok(authors.ToGetAuthorResponses());
});

authors.MapGet("/{id}", async Task<Results<NotFound, Ok<GetAuthorResponse>>> (
    long id, CancellationToken token, ApplicationDbContext context) =>
{
    var author = await context.Authors.FindAsync([id], token);

    if (author is null) return TypedResults.NotFound();

    var authorResponse = author.ToAuthorResponse();

    return TypedResults.Ok(authorResponse);
});

authors.MapPost("/", async Task<Results<BadRequest<ErrorResponse>, Created<GetAuthorResponse>>> (
    [FromBody] AuthorRequest request, CancellationToken token, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return TypedResults.BadRequest(new ErrorResponse("Validation failed", errors));
    }

    var author = request.ToAuthor();

    context.Authors.Add(author);
    await context.SaveChangesAsync(token);

    var authorResponse = author.ToAuthorResponse();

    return TypedResults.Created($"/authors/{authorResponse.Id}", authorResponse);
});

authors.MapPut("/{id}", async Task<Results<BadRequest<ErrorResponse>, NotFound, NoContent>> (
    long id, [FromBody] AuthorRequest request, CancellationToken token, ApplicationDbContext context) =>
{
    var validationResults = new List<ValidationResult>();

    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "Error",
                    v => new string[] { v.ErrorMessage! });

        return TypedResults.BadRequest(new ErrorResponse("Validation failed", errors));
    }

    var author = await context.Authors.FindAsync([id], token);

    if (author is null) return TypedResults.NotFound();

    author.FirstName = request.FirstName;
    author.LastName = request.LastName;

    await context.SaveChangesAsync(token);

    return TypedResults.NoContent();
});

authors.MapDelete("/{id}", async Task<Results<NotFound, NoContent>> (
    long id, CancellationToken token, ApplicationDbContext context) =>
{
    var author = await context.Authors.FindAsync([id], token);
    if (author is null) return TypedResults.NotFound();

    context.Authors.Remove(author);
    await context.SaveChangesAsync(token);

    return TypedResults.NoContent();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.Run();