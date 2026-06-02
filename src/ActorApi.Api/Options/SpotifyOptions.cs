namespace ActorApi.Api.Options;

public sealed class SpotifyOptions
{
    public const string SectionName = "Providers:Spotify";

    public bool Enabled { get; set; }

    public string BaseUrl { get; set; } = "https://api.spotify.com";

    public string AuthBaseUrl { get; set; } = "https://accounts.spotify.com";

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }
}