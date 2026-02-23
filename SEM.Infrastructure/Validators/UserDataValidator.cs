using FluentValidation;
using SEM.Domain.DTOs;

namespace SEM.Infrastructure.Validators;

public class UserDataValidator : AbstractValidator<UserAuthRequest>
{
    public UserDataValidator()
    {
        RuleFor(u => u.Email).EmailAddress().NotEmpty().WithMessage("Email is required");
        RuleFor(u => u.Password).NotEmpty().WithMessage("Password is required");
    }
}