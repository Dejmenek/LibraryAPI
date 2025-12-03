# LibraryAPI

## Table of Contents
- [General Information](#general-information)
- [Used Technologies](#used-technologies)
- [API Endpoints](#api-endpoints)
  - [Books](#books)
  - [Authors](#authors)
- [Authors](#authors)

## General Information

LibraryAPI is an academic project designed to demonstrate the implementation of a RESTful API for managing books, authors, and book copies in a library system. The solution is built using modern C# and .NET technologies, following best practices for API design, validation, and data modeling. This project is intended for educational purposes and may serve as a reference for similar academic or learning scenarios.

## Used Technologies

- C# 13.0
- .NET 9
- ASP.NET Core Minimal APIs
- Entity Framework Core
- SQL Server
- OpenAPI (Swagger) for API documentation

## API Endpoints

### Books

- `GET /books`
  - Retrieves all books. Optional query parameter `authorId` filters books by author.
- `GET /books/{id}`
  - Retrieves a specific book by its ID.
- `POST /books`
  - Creates a new book. Requires a JSON body with `Title`, `Year`, and `AuthorId`.
- `PUT /books/{id}`
  - Updates an existing book by ID. Requires a JSON body with updated `Title`, `Year`, and `AuthorId`.
- `DELETE /books/{id}`
  - Deletes a book by its ID.

### Authors

- `GET /authors`
  - Retrieves all authors.
- `GET /authors/{id}`
  - Retrieves a specific author by ID.
- `POST /authors`
  - Creates a new author. Requires a JSON body with `FirstName` and `LastName`.
- `PUT /authors/{id}`
  - Updates an existing author by ID. Requires a JSON body with updated `FirstName` and `LastName`.
- `DELETE /authors/{id}`
  - Deletes an author by its ID.

## Authors

- Damian Sempski 149720
