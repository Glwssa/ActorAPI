namespace ActorApi.Api.Options;

public sealed class OpenWeatherOptions
{
    public const string SectionName = "Providers:OpenWeather";

    public string BaseUrl { get; set; } = "http://api.openweathermap.org";

    public string? ApiKey { get; set; }
}


