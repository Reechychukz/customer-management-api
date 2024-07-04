using System.Text.RegularExpressions;
using FluentValidation;
using Application.DTOs;

namespace Application.Validations
{
	public class UserValidator: AbstractValidator<UserSignupDto>
	{
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty().NotNull().WithMessage("Phone number is required.").MinimumLength(10).WithMessage("PhoneNumber must not be less than 10 characters.")
           .MaximumLength(20).WithMessage("PhoneNumber must not exceed 50 characters.");
            RuleFor(x => x.Password).Matches(@"(?-i)(?=^.{8,}$)((?!.*\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\d){1,})|(?=(.*\W){1,}))^.*$")
                .WithMessage(@"Password must be at least 8 characters, at least 1 upper case letters (A – Z), Atleast 1 lower case letters (a – z), Atleast 1 number (0 – 9) or non-alphanumeric symbol (e.g. @ '$%£! ')");
        }
    }

    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}

