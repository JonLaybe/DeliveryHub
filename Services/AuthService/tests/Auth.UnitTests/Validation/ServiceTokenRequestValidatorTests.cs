using Auth.Api.Controllers;
using Auth.Api.Validation;
using FluentValidation.TestHelper;

namespace Auth.UnitTests.Validation;

public sealed class ServiceTokenRequestValidatorTests
{
    private readonly ServiceTokenRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ClientId_Is_Empty()
    {
        var request = new ServiceTokenController.ServiceTokenRequest(
            "",
            "secret",
            "discount.read"
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ClientId)
            .WithErrorMessage("ClientId is required.");
    }

    [Fact]
    public void Should_Have_Error_When_ClientSecret_Is_Empty()
    {
        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "",
            "discount.read"
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ClientSecret)
            .WithErrorMessage("ClientSecret is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Scope_Is_Longer_Than_500_Characters()
    {
        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            new string('a', 501)
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Scope)
            .WithErrorMessage("Scope must be less than 500 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            "discount.read profile.read"
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Scope_Is_Empty()
    {
        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            ""
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Scope_Is_Null()
    {
        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            null
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}