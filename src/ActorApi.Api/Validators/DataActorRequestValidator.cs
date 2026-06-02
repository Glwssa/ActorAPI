using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Extensions;
using FluentValidation;

namespace ActorApi.Api.Validators;

public sealed class DataActorRequestValidator : AbstractValidator<DataActorRequest>
{
    public DataActorRequestValidator()
    {
        RuleFor(request => request.ClientSelection)
            .IsInEnum()
            .WithMessage("ClientSelection must be a supported provider.");

        RuleFor(request => request.Parameters)
            .NotNull()
            .WithMessage("Parameters dictionary is required.");

        RuleFor(request => request.Headers)
            .NotNull()
            .WithMessage("Headers dictionary is required.");

        When(
            request => request.ClientSelection == ClientSelection.OpenWeather,
            () =>
            {
                RuleFor(request => request.Parameters)
                    .Must(parameters =>
                    {
                        var city = parameters?.GetValueOrDefaultIgnoreCase(
                            RequestParameterKeys.City);

                        return !string.IsNullOrWhiteSpace(city);
                    })
                    .WithMessage("OpenWeather requires parameter 'city'.");
            });
    }
}