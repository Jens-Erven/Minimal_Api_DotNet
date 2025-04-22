using FluentValidation;
using FluentValidation.Results;
using Library.Api.Auth;
using Library.Api.Data;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// I could add a different configuration here 
//builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "An API for managing a library of books",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });
});

builder.Services.AddSingleton<IDbConnnectionFactory>(_ => new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IBookService, BookService>();
// we will look in the entire assembly for all the validators, without having to specify them
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
// use authorization middleware here


app.MapPost("/books",
//[Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName)]
async (IBookService bookService, [FromBody] Book book, IValidator<Book> validator) =>
{
    var validationResults = await validator.ValidateAsync(book);
    if (!validationResults.IsValid)
    {
        return Results.BadRequest(validationResults.Errors);

    }

    var created = await bookService.CreateAsync(book);
    if (!created) return Results.BadRequest(new List<ValidationFailure> { new("Isbn", "Book with this ISBN already exists") });

    return Results.Created($"/books/{book.Isbn}", book);

});//.AllowAnomymous();

app.MapGet("/books", async (IBookService bookService, [FromQuery] string? searchTerm) =>
{
    if (searchTerm != null)
    {
        var matchesBooks = await bookService.SearchByTitleAsync(searchTerm);
        return Results.Ok(matchesBooks);
    }

    // no search term
    var books = await bookService.GetALlAsync();
    return Results.Ok(books);
});

app.MapGet("books/{isbn}", async (string isbn, IBookService bookService) =>
{
    var book = await bookService.GetByIsbnAsync(isbn);
    return book != null ? Results.Ok(book) : Results.NotFound();
});

app.MapPut("books/{isbn}", async (string isbn, Book book, IBookService bookService, IValidator<Book> validator) =>
{
    book.Isbn = isbn;
    var validationResult = await validator.ValidateAsync(book);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var updated = await bookService.UpdateAsync(book);
    return updated ? Results.Ok(book) : Results.NotFound();
});

app.MapDelete("books/{isbn}", async (string isbn, IBookService bookService) =>
{
    var deleted = await bookService.DeleteAsync(isbn);
    return deleted ? Results.NoContent() : Results.NotFound();
});



// Db initializes runs right before the applications runs
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();

