namespace ActorApi.Api.Options;

public sealed class NewsOptions
{
    public const string SectionName = "Providers:News";

    public bool Enabled { get; set; }

    public string BaseUrl { get; set; } = "https://newsapi.org";

    public string? ApiKey { get; set; }
}


