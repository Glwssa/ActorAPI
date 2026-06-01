using ActorApi.Api.Clients;
using ActorApi.Api.Contracts;

namespace ActorApi.Api.Services.Resolvers
{
    public interface IActorProviderClientResolver
    {
        IActorProviderClient Resolve(ClientSelection clientSelection);
    }
}
