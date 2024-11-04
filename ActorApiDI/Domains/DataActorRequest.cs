using System.ComponentModel.DataAnnotations;

namespace ActorApiDI.Domains
{
    /// <summary>
    /// Data Request Model
    /// </summary>
    public class DataActorRequest
    {
        public required ClientSelection clientSelection {  get; init; }
        public string? Param1 { get; set; }
        public string? Param2 { get; set; }
        public string? Header1 { get; set; }
        public string? Header2 { get; set; }
    }
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
