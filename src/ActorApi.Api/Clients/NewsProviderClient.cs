using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Exceptions;
using ActorApi.Api.Extensions;
using ActorApi.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ActorApi.Api.Clients
{
    public class NewsProviderClient(IHttpClientFactory _httpClientFactory, IOptionsSnapshot<NewsOptions> _options, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.News;

        /// <summary>
        /// Retrives News data from a specific keyword that was published today.
        /// NOTE: This Client is depricated and does not work.
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = Article Keyword, Header1 = ApiKey)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request, CancellationToken cancellationToken = default)
        {
            var query = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.Query);

            var country = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.Country);

            var language = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.Language);

            var apiKeyFromRequest = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ApiKey);

            var apiKeyFromConfiguration = _options.Value.ApiKey;

            var apiKey = !string.IsNullOrWhiteSpace(apiKeyFromRequest)
                ? apiKeyFromRequest
                : apiKeyFromConfiguration;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ProviderConfigurationException(
                    ClientSelection.News,
                    "News requires an API key. Provide it through user-secrets, environment variables, or the request headers dictionary using key 'apiKey'.");
            }

            DateTime dateToday = DateTime.Now;
            string requestUri = $"/v2/everything?q={query}&from={dateToday.ToString("yyyy-MM-dd")}&sortBy=publishedAt&apiKey={apiKey}";

            //caching check
            if (_memoryCache.TryGetValue($"News {requestUri}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            var client = _httpClientFactory.CreateClient("NewsClient");
            var response = await client.GetAsync(requestUri, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ProviderUnavailableException(
                    ClientSelection.News,
                    $"Received status code {(int)response.StatusCode}.");
            }

            var stringResult = await response.Content.ReadAsStringAsync(
                cancellationToken);

            return new DataActorResponse
            {
                ApiName = ProviderNames.News,
                Url = client.BaseAddress + requestUri,
                Body = stringResult
            };
        }
    }
}
