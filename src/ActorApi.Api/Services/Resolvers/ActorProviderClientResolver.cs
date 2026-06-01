using ActorApi.Api.Clients;
using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;

namespace ActorApi.Api.Services.Resolvers
{
    public class ActorProviderClientResolver : IActorProviderClientResolver
    {
        private readonly IReadOnlyDictionary<ClientSelection, IActorProviderClient> _clients;
        private readonly IConfiguration _configuration;

        public ActorProviderClientResolver(IEnumerable<IActorProviderClient> clients,
        IConfiguration configuration)
        {
            _clients = clients.ToDictionary(client => client.ClientSelection);
            _configuration = configuration;
        }

        public IActorProviderClient Resolve(ClientSelection clientSelection)
        {
            if (clientSelection == ClientSelection.Spotify)
            {
                var spotifyEnabled = _configuration.GetValue<bool>("Providers:Spotify:Enabled");

                if (!spotifyEnabled)
                {
                    throw new BadHttpRequestException(
                        "Spotify provider is currently disabled because it requires authentication refactoring.",
                        400);
                }
            }

            if (_clients.TryGetValue(clientSelection, out var client))
            {
                return client;
            }

            throw new BadHttpRequestException(
                $"Provider '{clientSelection}' is not supported.",
                400);
        }
    }
}
