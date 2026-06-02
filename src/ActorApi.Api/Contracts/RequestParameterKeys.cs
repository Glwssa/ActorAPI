namespace ActorApi.Api.Contracts
{
    /// <summary>
    /// Known provider parameter keys supported by the generic request contract.
    /// </summary>
    public static class RequestParameterKeys
    {
        /// <summary>
        /// City name used by the OpenWeather provider. Example: "Athens".
        /// </summary>
        public const string City = "city";

        /// <summary>
        /// Unit system used by OpenWeather. Example: "metric".
        /// </summary>
        public const string Units = "units";

        /// <summary>
        /// Search query used by providers such as News or Spotify.
        /// </summary>
        public const string Query = "query";

        /// <summary>
        /// Country code used by News provider. Example: "us".
        /// </summary>
        public const string Country = "country";

        /// <summary>
        /// Language code used by News provider. Example: "en".
        /// </summary>
        public const string Language = "language";
        /// <summary>
        /// Article code used by News provider. Example: "".
        /// </summary>
        public const string Article = "Article";
        /// <summary>
        /// Search query used by providers such as News or Spotify.
        /// </summary>
        public const string ArtistId = "ArtistId";
    }
}
