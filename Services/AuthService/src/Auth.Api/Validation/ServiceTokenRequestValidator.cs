using Auth.Api.Controllers;
using FluentValidation;

namespace Auth.Api.Validation;

public sealed class ServiceTokenRequestValidator
    : AbstractValidator<ServiceTokenController.ServiceTokenRequest>
{
    public ServiceTokenRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("ClientId is required.");

        RuleFor(x => x.ClientSecret)
            .NotEmpty()
            .WithMessage("ClientSecret is required.");

        RuleFor(x => x.Scope)
            .MaximumLength(500)
            .WithMessage("Scope must be less than 500 characters.");
    }
}