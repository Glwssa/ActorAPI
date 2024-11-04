using ActorApiDI.Domains;
using Microsoft.AspNetCore.Mvc;

namespace ActorApiDI.Clients
{
    public interface IDataActorClient
    {
        Task<DataActorResponse> GetData(DataActorRequest request);
    }
}
