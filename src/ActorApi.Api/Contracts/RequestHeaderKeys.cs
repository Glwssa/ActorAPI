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
        public const string ApiKey = "ApiKey";
        /// <summary>
        /// ClientID for Spotify Auth.
        /// </summary>
        public const string ClientID = "ClientID";
        /// <summary>
        /// ClientSecret for Spotify Auth.
        /// </summary>
        public const string ClientSecret = "ClientSecret";
    }
}
