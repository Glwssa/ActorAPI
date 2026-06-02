using ActorApi.Api.Domains;

namespace ActorApi.Api.Contracts
{
    /// <summary>
    /// Generic request used to call one of the supported external provider clients.
    /// </summary>
    /// <remarks>
    /// Supported client selections:
    /// - CatFacts: no parameters required.
    /// - CoinDesk: currently disabled because the existing API endpoint is unavailable or unreliable.
    /// - OpenWeather: parameters may include "city" and "units"; headers may include "apiKey".
    /// - News: parameters may include "query", "country", and "language"; headers may include "apiKey". (Currently disabled)
    /// - Spotify: currently disabled because authentication is pending refactoring.
    /// </remarks>
    public class DataActorRequest
    {
        /// <summary>
        /// The external provider client that should handle the request.
        /// </summary>
        /// <example>CatFacts</example>
        public ClientSelection ClientSelection { get; set; }
        /// <summary>
        /// Provider-specific query parameters.
        /// </summary>
        /// <remarks>
        /// Examples:
        /// OpenWeather: { "city": "Athens", "units": "metric" }
        /// News: { "query": "technology", "country": "us", "language": "en" }
        /// </remarks>
        public Dictionary<string, string> Parameters { get; set; } = [];
        /// <summary>
        /// Provider-specific header or credential overrides for local Swagger testing.
        /// </summary>
        /// <remarks>
        /// Example:
        /// { "apiKey": "your-local-api-key" }
        /// </remarks>
        public Dictionary<string, string> Headers { get; set; } = [];
    }
}
