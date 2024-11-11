using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class RoomValidator : AbstractValidator<Room>
{
    public RoomValidator()
    {
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(0).WithMessage("Capacity must be greater than or equal to 0.");
    }
}