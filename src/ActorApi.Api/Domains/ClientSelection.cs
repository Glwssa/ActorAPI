using System.ComponentModel.DataAnnotations;

namespace ActorApi.Api.Domains
{
    /// <summary>
    /// List of available Clients
    /// </summary>
    public enum ClientSelection
    {
        [Display(Name = "OpenWeather")]
        OpenWeather = 0,
        [Display(Name = "CatFacts")]
        CatFacts = 1,
        [Display(Name = "Spotify")]
        Spotify = 2,
        [Display(Name = "News")]
        News = 3,
        [Display(Name = "CoinDesk")]
        CoinDesk = 4,

    }
}
