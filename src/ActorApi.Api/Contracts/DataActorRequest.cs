using ActorApi.Api.Domains;

namespace ActorApi.Api.Contracts
{
    /// <summary>
    /// Data Request Model
    /// </summary>
    public class DataActorRequest
    {
        public required ClientSelection ClientSelection {  get; init; }
        public string? Param1 { get; set; }
        public string? Param2 { get; set; }
        public string? Header1 { get; set; }
        public string? Header2 { get; set; }
    }
}
