using ActorApi.Api.Clients;
using ActorApi.Api.Domains;

namespace ActorApi.Api.Services
{
    /// <summary>
    /// Data Actor Service that manages all the Client requests
    /// </summary>
    /// <param name="_WeatherDataActorClient">OpenWeather Client</param>
    /// <param name="_CatFactsDataActorClient">Cat Facts Client</param>
    /// <param name="_NewsDataActorClient">News Client</param>
    /// <param name="_SpotifyDataActorClient">Spotify Client</param>
    /// <param name="_CoinDeskDataActorClient"> Coin Desk Client</param>
    public class ActorService(
        [FromKeyedServices("Weather")] IActorProviderClient _WeatherDataActorClient,
        [FromKeyedServices("CatFacts")] IActorProviderClient _CatFactsDataActorClient,
        [FromKeyedServices("News")] IActorProviderClient _NewsDataActorClient,
        [FromKeyedServices("Spotify")] IActorProviderClient _SpotifyDataActorClient,
        [FromKeyedServices("CoinDesk")] IActorProviderClient _CoinDeskDataActorClient,
        IConfiguration _configuration) : IActorService
    {
        public Task<DataActorResponse> GetData(DataActorRequest request)
        {
            if((request is not null))
            {
                //Switch between available clients
                switch (request.clientSelection)
                {
                    case ClientSelection.OpenWeather:
                        return _WeatherDataActorClient.GetData(request);
                    case ClientSelection.CatFacts:
                        return _CatFactsDataActorClient.GetData(request);
                    case ClientSelection.Spotify:
                        var spotifyEnabled = _configuration.GetValue<bool>("Providers:Spotify:Enabled");

                        if (!spotifyEnabled)
                        {
                            throw new BadHttpRequestException(
                                "Spotify provider is currently disabled because it requires authentication refactoring.",
                                400);
                        }

                        return _SpotifyDataActorClient.GetData(request);
                    case ClientSelection.News:
                        return _NewsDataActorClient.GetData(request);
                    case ClientSelection.CoinDesk:
                        return _CoinDeskDataActorClient.GetData(request);
                    default:
                        throw new BadHttpRequestException("Error: client Selection field is not set.", 400);
                }
            }
            else
            {
                throw new BadHttpRequestException("Error: DataRequest field is invalid.", 400);
            }
            
            
        }
    }
}
