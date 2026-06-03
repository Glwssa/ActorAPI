using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Exceptions;
using ActorApi.Api.Extensions;
using ActorApi.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ActorApi.Api.Clients
{

    public class SpotifyProviderClient(IHttpClientFactory _httpClientFactory, IOptionsSnapshot<SpotifyOptions> _options, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.Spotify;

        /// <summary>
        /// Retrives information about the requested artist 
        /// </summary>
        /// <param name="request">getRequest Parameters (Param1 = ArtistId, Header1 = ClientID, Header2 = ClientSecret)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request, CancellationToken cancellationToken = default)
        {
            //Valid fields check
            var artistId = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.ArtistId);
            var clientIdFromRequest = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ClientID);
            var clientSecretFromRequest = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ClientSecret);

            var clientIdFromConfiguration = _options.Value.ClientId;
            var clientSecretFromConfiguration = _options.Value.ClientSecret;

            var clientId = !string.IsNullOrWhiteSpace(clientIdFromRequest)
                ? clientIdFromRequest
                : clientIdFromConfiguration;

            var clientSecret = !string.IsNullOrWhiteSpace(clientSecretFromRequest)
                ? clientSecretFromRequest
                : clientSecretFromConfiguration;

            if (string.IsNullOrWhiteSpace(artistId))
            {
                throw new ProviderConfigurationException(
                    ClientSelection.Spotify,
                    "Spotify requires parameter 'artistId'.");
            }

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ProviderConfigurationException(
                    ClientSelection.Spotify,
                    "Spotify requires client credentials. Provide them through user-secrets, environment variables, or the request headers dictionary using keys 'clientId' and 'clientSecret'.");
            }

            //Authentication HttpRequest setup
            var authClient = _httpClientFactory.CreateClient("SpotifyAuthClient");
            var authRequest = new HttpRequestMessage(new HttpMethod("POST"), "/api/token");
            authRequest.Content = new StringContent($"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}");
            authRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var authResponse = await authClient.SendAsync(authRequest, cancellationToken);

            //Success authentication response check
            if (!authResponse.IsSuccessStatusCode)
            {
                throw new ProviderUnavailableException(
                    ClientSelection.Spotify,
                    $"Authentication request failed with status code {(int)authResponse.StatusCode}.");
            }
            var authStringResult = await authResponse.Content.ReadAsStringAsync(cancellationToken);
            var auth = JsonConvert.DeserializeObject<SpotifyAuthResponse>(authStringResult);

            //Caching check
            string url = $"/v1/artists/{artistId}";
            if (_memoryCache.TryGetValue($"Spotify {url}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //getRequest
            var client = _httpClientFactory.CreateClient("SpotifyClient");
            var getRequest = new HttpRequestMessage(new HttpMethod("GET"), url);
            getRequest.Headers.TryAddWithoutValidation("Authorization", $"{auth!.token_type}  {auth!.access_token}");

            var response = await client.SendAsync(getRequest, cancellationToken);

            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new ProviderUnavailableException(
                ClientSelection.Spotify,
                $"Error: Spotify Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync(cancellationToken);
            //return retrived data in generic format and cache it
            var cachedResult = new DataActorResponse()
            {
                ApiName = ProviderNames.Spotify,
                Url = client.BaseAddress + url,
                Body = stringResult

            };
            _memoryCache.Set($"Spotify {url}", cachedResult);

            return cachedResult;
        }
    }
}
