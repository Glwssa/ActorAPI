using ActorApi.Api.Clients;
using ActorApi.Api.Domains;
using ActorApi.Api.Exceptions;
using ActorApi.Api.Options;
using Microsoft.Extensions.Options;

namespace ActorApi.Api.Services.Resolvers
{
    public class ActorProviderClientResolver : IActorProviderClientResolver
    {
        private readonly IOptionsSnapshot<SpotifyOptions> _spotifyOptions;
        private readonly IOptionsSnapshot<NewsOptions> _newsOptions;
        private readonly IOptionsSnapshot<CoinDeskOptions> _coinDeskOptions;
        private readonly Dictionary<ClientSelection, IActorProviderClient> _clients;

        public ActorProviderClientResolver(
            IEnumerable<IActorProviderClient> clients,
            IOptionsSnapshot<SpotifyOptions> spotifyOptions,
            IOptionsSnapshot<NewsOptions> newsOptions,
            IOptionsSnapshot<CoinDeskOptions> coinDeskOptions)
        {
            _clients = clients.ToDictionary(client => client.ClientSelection);
            _spotifyOptions = spotifyOptions;
            _newsOptions = newsOptions;
            _coinDeskOptions = coinDeskOptions;
        }

        public IActorProviderClient Resolve(ClientSelection clientSelection)
        {
            if (clientSelection == ClientSelection.Spotify)
            {
                var spotifyEnabled = _spotifyOptions.Value.Enabled;

                if (!spotifyEnabled)
                {
                    throw new ProviderDisabledException(
                        ClientSelection.Spotify,
                        "it requires authentication refactoring");
                }
            }
            else if (clientSelection == ClientSelection.News)
            {
                var newsEnabled = _newsOptions.Value.Enabled;
                if (!newsEnabled)
                {
                    throw new ProviderDisabledException(
                        ClientSelection.News,
                        "the existing provider integration is no longer reliable");
                }
            }
            else if (clientSelection == ClientSelection.CoinDesk && !_coinDeskOptions.Value.Enabled)
            {
                throw new ProviderDisabledException(
                    ClientSelection.CoinDesk,
                    "the existing API endpoint is unavailable or unreliable");
            }


            if (_clients.TryGetValue(clientSelection, out var client))
            {
                return client;
            }

            throw new ProviderNotSupportedException(clientSelection);
        }
    }
}



