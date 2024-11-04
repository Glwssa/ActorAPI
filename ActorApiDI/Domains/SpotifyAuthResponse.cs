namespace ActorApiDI.Domains
{
    /// <summary>
    /// Spotify Authentication Response Model
    /// </summary>
    public class SpotifyAuthResponse
    {
        public string access_token {  get; set; }
        public string token_type {  get; set; }
        public decimal expires_in {  get; set; }
    }
}
