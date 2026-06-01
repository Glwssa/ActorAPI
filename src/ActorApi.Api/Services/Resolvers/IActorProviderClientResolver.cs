using ActorApi.Api.Clients;
using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;

namespace ActorApi.Api.Services.Resolvers
{
    public interface IActorProviderClientResolver
    {
        IActorProviderClient Resolve(ClientSelection clientSelection);
    }
}
