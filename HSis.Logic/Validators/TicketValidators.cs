using FluentValidation;
using HSis.Logic.DTOs;

namespace HSis.Logic.Validators
{
    public class TicketCreateValidator : AbstractValidator<TicketCreateDto>
    {
        public TicketCreateValidator()
        {
            RuleFor(x => x.IdUsuario)
                .GreaterThan(0).WithMessage("El ID de usuario es obligatorio.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción del problema no puede estar vacía.")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres.")
                .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");
        }
    }

    public class TicketUpdateValidator : AbstractValidator<TicketUpdateDto>
    {
        public TicketUpdateValidator()
        {
            RuleFor(x => x.IdTicket)
                .GreaterThan(0).WithMessage("El ID del ticket es inválido.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("El estatus es obligatorio.");

            RuleFor(x => x.Solucion)
                .NotEmpty()
                .When(x => x.Status == "Cerrado")
                .WithMessage("Debe ingresar una solución antes de cerrar el ticket.");
        }
    }
}
