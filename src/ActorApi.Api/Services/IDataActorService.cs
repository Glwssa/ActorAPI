using ActorApi.Api.Domains;
using Microsoft.AspNetCore.Mvc;

namespace ActorApi.Api.Services
{
    public interface IDataActorService
    {
        Task<DataActorResponse> GetData(DataActorRequest request);
    }
}
