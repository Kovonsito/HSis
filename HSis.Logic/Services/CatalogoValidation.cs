using FluentValidation;

namespace HSis.Logic.Services;

internal static class CatalogoValidation
{
    public static async Task ValidarAsync<T>(IValidator<T>? validator, T model)
    {
        if (validator is null)
        {
            return;
        }

        var resultado = await validator.ValidateAsync(model);
        if (!resultado.IsValid)
        {
            throw new ValidationException(resultado.Errors);
        }
    }
}
