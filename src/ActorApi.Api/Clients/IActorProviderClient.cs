using ActorApi.Api.Domains;

namespace ActorApi.Api.Clients
{
    public interface IActorProviderClient
    {
        ClientSelection ClientSelection { get; }

        Task<DataActorResponse> GetDataAsync(DataActorRequest request);
    }
}
