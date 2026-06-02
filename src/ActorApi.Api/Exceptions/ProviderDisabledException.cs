using ActorApi.Api.Domains;

namespace ActorApi.Api.Exceptions;

public sealed class ProviderDisabledException(
    ClientSelection clientSelection,
    string reason) : ActorApiException(
        $"{clientSelection} provider is currently disabled because {reason}.",
        StatusCodes.Status400BadRequest,
        "Provider disabled")
{
    public ClientSelection ClientSelection { get; } = clientSelection;

    public string Reason { get; } = reason;
}