using FluentValidation;
using HSis.Data.Models;
using HSis.Contracts.DTOs;

namespace HSis.Logic.Validators
{
    public class UsuarioValidator : AbstractValidator<UsuarioDto>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

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
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

            RuleFor(x => x.UnidadMedida)
                .NotEmpty().WithMessage("Debe seleccionar una unidad de medida.")
                .MaximumLength(20).WithMessage("La unidad de medida no puede exceder los 20 caracteres.");
        }
    }

    public class DepartamentoValidator : AbstractValidator<Departamento>
    {
        public DepartamentoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del departamento es obligatorio.")
                .MaximumLength(30).WithMessage("El nombre no puede exceder los 30 caracteres.");
            RuleFor(x => x.Descripcion)
                .MaximumLength(250).WithMessage("La descripción no puede exceder los 250 caracteres.");
        }
    }

    public class EmpresaValidator : AbstractValidator<Empresa>
    {
        public EmpresaValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la empresa es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
            RuleFor(x => x.Calle)
                .MaximumLength(50).WithMessage("La calle no puede exceder los 50 caracteres.");
            RuleFor(x => x.Numero)
                .MaximumLength(10).WithMessage("El número no puede exceder los 10 caracteres.");
            RuleFor(x => x.Colonia)
                .MaximumLength(50).WithMessage("La colonia no puede exceder los 50 caracteres.");
            RuleFor(x => x.Telefono)
                .MaximumLength(15).WithMessage("El teléfono no puede exceder los 15 caracteres.")
                .Matches(@"^\(\d{3}\) \d{3}-\d{4}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Telefono))
                .WithMessage("El formato del teléfono debe ser (XXX) XXX-XXXX.");
        }
    }

    public class SucursalValidator : AbstractValidator<Sucursal>
    {
        public SucursalValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la sucursal es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
            RuleFor(x => x.Calle)
                .MaximumLength(30).WithMessage("La calle no puede exceder los 30 caracteres.");
            RuleFor(x => x.Numero)
                .MaximumLength(10).WithMessage("El número no puede exceder los 10 caracteres.");
            RuleFor(x => x.Colonia)
                .MaximumLength(50).WithMessage("La colonia no puede exceder los 50 caracteres.");
            RuleFor(x => x.Telefono)
                .MaximumLength(15).WithMessage("El teléfono no puede exceder los 15 caracteres.")
                .Matches(@"^\(\d{3}\) \d{3}-\d{4}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Telefono))
                .WithMessage("El formato del teléfono debe ser (XXX) XXX-XXXX.");
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
                .MaximumLength(20).WithMessage("El nombre del puesto no puede exceder los 20 caracteres.");
            RuleFor(x => x.Descripcion)
                .MaximumLength(250).WithMessage("La descripción no puede exceder los 250 caracteres.");
        }
    }

    public class RolUsuarioValidator : AbstractValidator<RolUsuario>
    {
        public RolUsuarioValidator()
        {
            RuleFor(x => x.Descripcion)
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

