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

books.MapGet("/", async (ApplicationDbContext context) =>
{
    var books = await context.Books.Include(b => b.Author).AsNoTracking().ToListAsync();
    return books.ToBooksWithAuthorsResponse();
});

books.MapPost("/{id}", async (int id, [FromBody] BookRequest request, ApplicationDbContext context) =>
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

    var book = request.ToBook();

    context.Books.Add(book);
    await context.SaveChangesAsync();

    return Results.Created($"/books/{book.Id}", book);
});

books.MapPut("/{id}", async (int id, [FromBody] BookRequest request, ApplicationDbContext context) =>
{
    var book = await context.Books.FindAsync(id);

    if (book is null) return Results.NotFound();

    book.Year = request.Year;
    book.Title = request.Title;
    book.AuthorId = request.AuthorId;

    await context.SaveChangesAsync();

    return Results.NoContent();
});

books.MapDelete("/{id}", async (int id, ApplicationDbContext context) =>
{
    if (await context.Books.FindAsync(id) is Book book)
    {
        context.Books.Remove(book);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();