using ActorApi.Api.Domains;
using ActorApi.Api.Services.Resolvers;

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
    public sealed class ActorService : IActorService
    {
        private readonly IActorProviderClientResolver _clientResolver;
        public ActorService(IActorProviderClientResolver clientResolver)
        {
            _clientResolver = clientResolver;
        }
        public Task<DataActorResponse> GetDataAsync(DataActorRequest request)
        {
            var client = _clientResolver.Resolve(request.ClientSelection);

            return client.GetDataAsync(request);
        }
    }
}
