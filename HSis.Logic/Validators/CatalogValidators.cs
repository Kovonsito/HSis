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

    public class EmpresaValidator : AbstractValidator<Empresa>
    {
        public EmpresaValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la empresa es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
            RuleFor(x => x.Telefono)
                .MaximumLength(20).WithMessage("El teléfono no puede exceder los 20 caracteres.");
        }
    }

    public class SucursalValidator : AbstractValidator<Sucursal>
    {
        public SucursalValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la sucursal es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
            RuleFor(x => x.IdEmpresa)
                .NotNull().WithMessage("Debe seleccionar la empresa de la sucursal.")
                .GreaterThan(0).WithMessage("Debe seleccionar la empresa de la sucursal.");
        }
    }

    public class PuestoValidator : AbstractValidator<Puesto>
    {
        public PuestoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del puesto es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del puesto no puede exceder los 50 caracteres.");
        }
    }

    public class RolUsuarioValidator : AbstractValidator<RolUsuario>
    {
        public RolUsuarioValidator()
        {
            RuleFor(x => x.Descripción)
                .NotEmpty().WithMessage("La descripción del rol es obligatoria.")
                .MaximumLength(50).WithMessage("La descripción del rol no puede exceder los 50 caracteres.");
        }
    }

    public class MovimientoMaterialValidator : AbstractValidator<MovimientoMaterial>
    {
        public MovimientoMaterialValidator()
        {
            RuleFor(x => x.IdMaterial)
                .GreaterThan(0).WithMessage("Debe seleccionar un material.");
            RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
            RuleFor(x => x.CostoUnitario)
                .GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
            RuleFor(x => x.IdUsuario)
                .GreaterThan(0).WithMessage("Debe seleccionar un usuario responsable.");
            RuleFor(x => x.Motivo)
                .NotEmpty().WithMessage("Debe seleccionar o escribir un motivo.");
        }
    }
}
