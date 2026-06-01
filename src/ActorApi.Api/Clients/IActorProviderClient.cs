using ActorApi.Api.Domains;
using Microsoft.AspNetCore.Mvc;

namespace ActorApi.Api.Clients
{
    public interface IActorProviderClient
    {
        Task<DataActorResponse> GetData(DataActorRequest request);
    }
}
