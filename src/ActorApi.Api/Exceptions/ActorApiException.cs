namespace ActorApi.Api.Exceptions;

public abstract class ActorApiException(
    string message,
    int statusCode,
    string title) : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    public string Title { get; } = title;
}