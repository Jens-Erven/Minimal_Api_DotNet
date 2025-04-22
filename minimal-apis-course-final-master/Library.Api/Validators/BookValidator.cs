using FluentValidation;
using Library.Api.Models;

namespace Library.Api.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(book => book.Isbn)
                 .Matches(@"^(?=(?:[^0-9]*[0-9]){10}(?:(?:[^0-9]*[0-9]){3})?$)[\d-]+$")
                 .WithMessage("Invalid ISBN");
            RuleFor(book => book.Title).NotEmpty();
            RuleFor(book => book.Author).NotEmpty();
            RuleFor(book => book.ShortDescription).NotEmpty();
            RuleFor(book => book.PageCount).GreaterThan(0);
            RuleFor(book => book.Author).NotEmpty();
        }
    }
}
