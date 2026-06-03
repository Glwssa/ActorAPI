using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Exceptions;
using ActorApi.Api.Extensions;
using ActorApi.Api.Options;
using Microsoft.AspNetCore.WebUtilities;
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
            var city = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.City);

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ProviderConfigurationException(
                    ClientSelection.OpenWeather,
                    "OpenWeather requires parameter 'city'.");
            }

            var units = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.Units) ?? "metric";

            var apiKeyFromRequest = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ApiKey);

            var apiKeyFromConfiguration = _options.Value.ApiKey;

            var apiKey = !string.IsNullOrWhiteSpace(apiKeyFromRequest)
                ? apiKeyFromRequest
                : apiKeyFromConfiguration;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ProviderConfigurationException(
                    ClientSelection.OpenWeather,
                    "OpenWeather requires an API key. Provide it through user-secrets, environment variables, or the request headers dictionary using key 'apiKey'.");
            }

            var requestUri = QueryHelpers.AddQueryString("/data/2.5/weather",
                new Dictionary<string, string?>
                {
                    ["q"] = city,
                    ["appid"] = apiKey,
                    ["units"] = units
                });

            var cacheKey = CacheKeyExtensions.CreateProviderCacheKey(ProviderNames.OpenWeather, requestUri);

            if (_memoryCache.TryGetValue(cacheKey, out DataActorResponse? cachedResult)
                && cachedResult is not null)
            {
                return cachedResult;
            }

            var client = _httpClientFactory.CreateClient("OpenWeatherClient");

            var response = await client.GetAsync(requestUri, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ProviderUnavailableException(
                    ClientSelection.OpenWeather,
                    $"Received status code {(int)response.StatusCode}.");
            }

            var stringResult = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = new DataActorResponse
            {
                ApiName = ProviderNames.OpenWeather,
                Url = client.BaseAddress + requestUri,
                Body = stringResult
            };

            _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }
    }
}
