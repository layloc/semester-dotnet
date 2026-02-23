using FluentValidation;
using SEM.Domain.Entities;

namespace SEM.Infrastructure.Validators;

public class PropertyValidator : AbstractValidator<PropertyInfo>
{
    public PropertyValidator()
    {
        RuleFor(prop => prop.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(prop => prop.Type).NotEmpty().WithMessage("Type is required");
    }
}