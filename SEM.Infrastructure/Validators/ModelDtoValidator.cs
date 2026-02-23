using FluentValidation;
using SEM.Domain.DTOs;

namespace SEM.Infrastructure.Validators;

public class ModelDtoValidator : AbstractValidator<ModelDto>
{
    public ModelDtoValidator()
    {
        RuleFor(m => m.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(m => m.Properties).NotEmpty().WithMessage("Properties is required");
        RuleForEach(m => m.Properties).SetValidator(new PropertyValidator());
    }
}