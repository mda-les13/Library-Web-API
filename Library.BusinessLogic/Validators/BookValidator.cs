using FluentValidation;
using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Validators
{
    public class BookValidator : AbstractValidator<BookModel>
    {
        public BookValidator()
        {
            RuleFor(b => b.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Length(10, 13).WithMessage("ISBN must be between 10 and 13 characters.");

            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

            RuleFor(b => b.Genre)
                .NotEmpty().WithMessage("Genre is required.")
                .MaximumLength(100).WithMessage("Genre must not exceed 100 characters.");

            RuleFor(b => b.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(b => b.AuthorId)
                .GreaterThan(0).WithMessage("AuthorId must be greater than 0.");
        }
    }
}
