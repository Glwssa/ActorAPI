using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Extensions;
using ActorApi.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ActorApi.Api.Clients
{
    public class OpenWeatherProviderClient(IHttpClientFactory _httpClientFactory, IOptionsSnapshot<OpenWeatherOptions> _options, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.OpenWeather;

        /// <summary>
        /// Retrives weather data for the requested city.
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = city, Header 1 = ApiKey)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request, CancellationToken cancellationToken = default)
        {
            //Valid fields check
            var city = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.City);
            var apiKeyFromRequest = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ApiKey);
            var apiKeyFromConfiguration = _options.Value.ApiKey;

            var apiKey = !string.IsNullOrWhiteSpace(apiKeyFromRequest)
                ? apiKeyFromRequest
                : apiKeyFromConfiguration;

            if (city is null )
                throw new BadHttpRequestException("Error: Please provide all the required fields.", 400);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new BadHttpRequestException(
                    "OpenWeather requires an API key. Provide it through user-secrets, environment variables, or the request headers dictionary using key 'apiKey'.",
                    400);
            }

            //caching check
            string url = $"/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
            if (_memoryCache.TryGetValue($"OpenWeather {url}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //Setup HttpRequest
            var client = _httpClientFactory.CreateClient("OpenWeatherClient");
            var response = await client.GetAsync(url, cancellationToken);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: OpenWeather Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync(cancellationToken);

            //return retrived data in generic format and cache it
            var cachedResult = new DataActorResponse()
            {
                ApiName = "OpenWeather",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
            _memoryCache.Set($"OpenWeather {url}", cachedResult);

            return cachedResult;
        }
    }
}
