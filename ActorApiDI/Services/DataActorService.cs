
using ActorApiDI.Clients;
using ActorApiDI.Domains;
using Microsoft.AspNetCore.Mvc;

namespace ActorApiDI.Services
{
    /// <summary>
    /// Data Actor Service that manages all the Client requests
    /// </summary>
    /// <param name="_WeatherDataActorClient">OpenWeather Client</param>
    /// <param name="_CatFactsDataActorClient">Cat Facts Client</param>
    /// <param name="_NewsDataActorClient">News Client</param>
    /// <param name="_SpotifyDataActorClient">Spotify Client</param>
    /// <param name="_CoinDeskDataActorClient"> Coin Desk Client</param>
    public class DataActorService(
        [FromKeyedServices("Weather")] IDataActorClient _WeatherDataActorClient,
        [FromKeyedServices("CatFacts")] IDataActorClient _CatFactsDataActorClient,
        [FromKeyedServices("News")] IDataActorClient _NewsDataActorClient,
        [FromKeyedServices("Spotify")] IDataActorClient _SpotifyDataActorClient,
        [FromKeyedServices("CoinDesk")] IDataActorClient _CoinDeskDataActorClient) : IDataActorService
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
