using FluentValidation;
using ModuloNet.Application.Auth;

namespace ModuloNet.Application.Features.Auth.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role)
            .Must(r => r == AuthRoles.Parent || r == AuthRoles.Student)
            .WithMessage("Role must be Parent or Student.");
    }
}
