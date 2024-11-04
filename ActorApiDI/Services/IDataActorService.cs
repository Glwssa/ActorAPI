using ActorApiDI.Domains;
using Microsoft.AspNetCore.Mvc;

namespace ActorApiDI.Services
{
    public interface IDataActorService
    {
        Task<DataActorResponse> GetData(DataActorRequest request);
    }
}
