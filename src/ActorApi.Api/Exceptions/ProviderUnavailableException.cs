using ActorApi.Api.Domains;

namespace ActorApi.Api.Exceptions;

public sealed class ProviderUnavailableException : ActorApiException
{
    public ProviderUnavailableException(
        ClientSelection clientSelection,
        string? reason = null)
        : base(
            reason is null
                ? $"{clientSelection} provider is currently unavailable."
                : $"{clientSelection} provider is currently unavailable. {reason}",
            StatusCodes.Status503ServiceUnavailable,
            "Provider unavailable")
    {
        ClientSelection = clientSelection;
    }

    public ClientSelection ClientSelection { get; }
}