using FluentValidation;
using ModuloNet.Application.Auth;

namespace ModuloNet.Application.Features.Auth.ExternalExchange;

public sealed class ExternalExchangeValidator : AbstractValidator<ExternalExchangeCommand>
{
    public ExternalExchangeValidator()
    {
        RuleFor(x => x.Provider).NotEmpty().Must(p => p is "Google" or "Facebook");
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.Role)
            .Must(r => r == AuthRoles.Parent || r == AuthRoles.Student)
            .WithMessage("Role must be Parent or Student.");
    }
}
