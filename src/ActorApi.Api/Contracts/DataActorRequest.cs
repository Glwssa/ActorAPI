using ActorApi.Api.Domains;

namespace ActorApi.Api.Contracts
{
    /// <summary>
    /// Data Request Model
    /// </summary>
    public class DataActorRequest
    {
        public ClientSelection ClientSelection { get; set; }

        public Dictionary<string, string> Parameters { get; set; } = [];

        public Dictionary<string, string> Headers { get; set; } = [];
    }
}
