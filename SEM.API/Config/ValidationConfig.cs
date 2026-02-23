using FluentValidation;
using SEM.Infrastructure.Validators;

namespace SEM.API.Config;

public static class ValidationConfig 
{
    public static void ConfigureValidators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssemblyContaining<UserDataValidator>();
        serviceCollection.AddValidatorsFromAssemblyContaining<ModelDtoValidator>();
        serviceCollection.AddValidatorsFromAssemblyContaining<PropertyValidator>();
    }
}