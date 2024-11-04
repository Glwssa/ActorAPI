namespace ActorApiDI.Domains
{
    /// <summary>
    /// Generic Data Response Model
    /// </summary>
    public class DataActorResponse
    {
        public required string ApiName { get; set; }
        public string? Url { get; set; }
        public string? Body { get; set; }
    }
}
