namespace ActorApi.Api.Options;

public sealed class CatFactsOptions
{
    public const string SectionName = "Providers:CatFacts";

    public string BaseUrl { get; set; } = "https://catfact.ninja";
}


