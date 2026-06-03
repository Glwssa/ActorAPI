using ActorApi.Api.Domains;

namespace ActorApi.Api.Exceptions;

public sealed class ProviderConfigurationException : ActorApiException
{
    public ProviderConfigurationException(
        ClientSelection clientSelection,
        string message)
        : base(
            message,
            StatusCodes.Status400BadRequest,
            "Provider configuration error")
    {
        ClientSelection = clientSelection;
    }

    public ClientSelection ClientSelection { get; }
}