using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using Microsoft.Extensions.Caching.Memory;

namespace ActorApi.Api.Clients
{
    public class CatFactsProviderClient(IHttpClientFactory _httpClientFactory, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.CatFacts;

        /// <summary>
        /// Retrives random cat facts
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request, CancellationToken cancellationToken = default)
        {
            //Caching check
            if (_memoryCache.TryGetValue($"CatFacts {DateTime.Now.ToString("yyyyy-MM-dd")}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //Setup HttpRequest
            var client = _httpClientFactory.CreateClient("CatFacts");
            string url = $"/fact";
            var response = await client.GetAsync(url, cancellationToken);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: CatFacts Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync(cancellationToken);

            //return retrived data in generic format and add it to the cache
            var cachedResult = new DataActorResponse()
            {
                ApiName = "CatFacts",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
            _memoryCache.Set($"CatFacts {DateTime.Now.ToString("yyyyy-MM-dd")}", cachedResult);
            return cachedResult;
        }
    }
}
