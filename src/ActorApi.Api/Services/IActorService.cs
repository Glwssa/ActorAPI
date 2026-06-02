using ActorApi.Api.Contracts;

namespace ActorApi.Api.Services
{
    public interface IActorService
    {
        Task<DataActorResponse> GetDataAsync(DataActorRequest request,CancellationToken cancellationToken = default);
    }
}
