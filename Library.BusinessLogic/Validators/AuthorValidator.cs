using FluentValidation;
using Library.BusinessLogic.Models;

namespace Library.BusinessLogic.Validators
{
    public class AuthorValidator : AbstractValidator<AuthorModel>
    {
        public AuthorValidator()
        {
            RuleFor(a => a.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters.");

            RuleFor(a => a.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(100).WithMessage("LastName must not exceed 100 characters.");

            RuleFor(a => a.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

            RuleFor(a => a.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required.")
                .LessThan(DateTime.Now).WithMessage("DateOfBirth must be in the past.");
        }
    }
}
