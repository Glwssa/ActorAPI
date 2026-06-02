using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Extensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ActorApi.Api.Clients
{

    public class SpotifyProviderClient(IHttpClientFactory _httpClientFactory, IConfiguration _configuration, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.Spotify;

        /// <summary>
        /// Retrives information about the requested artist 
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = ArtistId, Header1 = ClientID, Header2 = ClientSecret)</param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request)
        {
            //Valid fields check
            var artistId = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.ArtistId);
            var clientIdFromRequest = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ClientID);
            var clientSecretFromRequest = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ClientSecret);

            var clientIdFromConfiguration = _configuration["Providers:SpotifyCridentials:ClientId"];
            var clientSecretFromConfiguration = _configuration["Providers:SpotifyCridentials:ClientSecret"];

            var clientId = !string.IsNullOrWhiteSpace(clientIdFromConfiguration)
                ? clientIdFromRequest
                : clientIdFromConfiguration;

            var clientSecret = !string.IsNullOrWhiteSpace(clientSecretFromConfiguration)
                ? clientSecretFromRequest
                : clientSecretFromConfiguration;

            if (artistId is null)
                throw new BadHttpRequestException("Error: Please provide all the required fields.", 400);

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new BadHttpRequestException(
                    "Spotify requires client ID . Provide it through user-secrets, environment variables, or the request headers dictionary using key 'clientId'.",
                    400);
            }
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new BadHttpRequestException(
                    "Spotify requires client secret. Provide it through user-secrets, environment variables, or the request headers dictionary using key 'clientSecret'.",
                    400);
            }

            //Authentication HttpRequest setup
            var authClient = _httpClientFactory.CreateClient("SpotifyAuthClient");
            var AuthRequest = new HttpRequestMessage(new HttpMethod("POST"), "/api/token");
            AuthRequest.Content = new StringContent($"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}");
            AuthRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var Authresponse = await authClient.SendAsync(AuthRequest);
            //Success authentication response check
            if (!Authresponse.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.UserAuthenticationError, "Error: Spotify Authentication Service was not successfull, please check your cridentials.");
            var authStringResult = await Authresponse.Content.ReadAsStringAsync();
            var auth = JsonConvert.DeserializeObject<SpotifyAuthResponse>(authStringResult);

            //Caching check
            string url = $"/v1/artists/{artistId}";
            if (_memoryCache.TryGetValue($"Spotify {url}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //Request
            var client = _httpClientFactory.CreateClient("SpotifyClient");
            var Request = new HttpRequestMessage(new HttpMethod("GET"), url);
            Request.Headers.TryAddWithoutValidation("Authorization", $"{auth!.token_type}  {auth!.access_token}");

            var response = await client.SendAsync(Request);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: Spotify Service was not available.");
            var stringResult = await response.Content.ReadAsStringAsync();
            //return retrived data in generic format and cache it
            var cachedResult = new DataActorResponse()
            {
                ApiName = "Spotify",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
            _memoryCache.Set($"Spotify {url}", cachedResult);

            return cachedResult;
        }
    }
}
