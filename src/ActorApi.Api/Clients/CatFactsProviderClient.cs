using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Exceptions;
using ActorApi.Api.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace ActorApi.Api.Clients
{
    public sealed class CatFactsProviderClient(IHttpClientFactory _httpClientFactory, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.CatFacts;
        private const string RequestUri = "/fact";

        /// <summary>
        /// Retrives random cat facts
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request, CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeyExtensions.CreateDailyProviderCacheKey(ProviderNames.CatFacts,DateOnly.FromDateTime(DateTime.UtcNow));

            if (_memoryCache.TryGetValue(cacheKey, out DataActorResponse? cachedResult) && cachedResult is not null)
            {
                return cachedResult;
            }

            var client = _httpClientFactory.CreateClient(ProviderNames.CatFacts);

            var response = await client.GetAsync(
                RequestUri,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ProviderUnavailableException(
                    ClientSelection.CatFacts,
                    $"Received status code {(int)response.StatusCode}.");
            }

            var stringResult = await response.Content.ReadAsStringAsync(
                cancellationToken);

            var result = new DataActorResponse
            {
                ApiName = ProviderNames.CatFacts,
                Url = client.BaseAddress + RequestUri,
                Body = stringResult
            };

            _memoryCache.Set(
                cacheKey,
                result,
                TimeSpan.FromHours(12));

            return result;
        }
    }
}
