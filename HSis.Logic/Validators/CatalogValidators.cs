using FluentValidation;
using HSis.Data.Models;

namespace HSis.Logic.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            // El modelo Usuario no tiene Email actualmente en la base de datos
            // RuleFor(x => x.Email)...

            RuleFor(x => x.Contraseña)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

            RuleFor(x => x.IdRol)
                .NotNull().WithMessage("Debe seleccionar un rol.")
                .GreaterThan(0).WithMessage("Debe seleccionar un rol.");
        }
    }

    public class MaterialValidator : AbstractValidator<Material>
    {
        public MaterialValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del material es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.UnidadMedida)
                .NotEmpty().WithMessage("Debe seleccionar una unidad de medida.");
        }
    }
    
    public class DepartamentoValidator : AbstractValidator<Departamento>
    {
        public DepartamentoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del departamento es obligatorio.");
        }
    }
}
