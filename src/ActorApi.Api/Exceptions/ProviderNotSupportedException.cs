using ActorApi.Api.Domains;

namespace ActorApi.Api.Exceptions;

public sealed class ProviderNotSupportedException(ClientSelection clientSelection) : ActorApiException(
        $"Provider '{clientSelection}' is not supported.",
        StatusCodes.Status400BadRequest,
        "Provider not supported")
{
    public ClientSelection ClientSelection { get; } = clientSelection;
}