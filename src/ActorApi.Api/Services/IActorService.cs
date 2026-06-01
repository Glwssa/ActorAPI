using ActorApi.Api.Domains;

namespace ActorApi.Api.Services
{
    public interface IActorService
    {
        Task<DataActorResponse> GetDataAsync(DataActorRequest request);
    }
}
