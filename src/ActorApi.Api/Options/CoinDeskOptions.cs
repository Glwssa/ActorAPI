namespace ActorApi.Api.Options;

public sealed class CoinDeskOptions
{
    public const string SectionName = "Providers:CoinDesk";

    public bool Enabled { get; set; }

    public string BaseUrl { get; set; } = "https://api.coindesk.com";
}