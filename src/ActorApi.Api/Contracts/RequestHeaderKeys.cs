namespace ActorApi.Api.Contracts
{
    /// <summary>
    /// Known provider header keys supported by the generic request contract.
    /// </summary>
    public static class RequestHeaderKeys
    {
        /// <summary>
        /// Optional per-request API key override for providers that require a key.
        /// </summary>
        public const string ApiKey = "apiKey";
        /// <summary>
        /// ClientID for Spotify Auth.
        /// </summary>
        public const string ClientID = "clientID";
        /// <summary>
        /// ClientSecret for Spotify Auth.
        /// </summary>
        public const string ClientSecret = "ClientSecret";
    }
}
