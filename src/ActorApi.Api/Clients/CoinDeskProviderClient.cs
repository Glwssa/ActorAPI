using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using Microsoft.Extensions.Caching.Memory;

namespace ActorApi.Api.Clients
{
    public class CoinDeskProviderClient(IHttpClientFactory _httpClientFactory, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.CoinDesk;

        /// <summary>
        /// Retrives the Bitcoin Price Index (BPI) in real-time
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request, CancellationToken cancellationToken = default)
        {
            //Check if cache has this entry and return it
            if (_memoryCache.TryGetValue($"CoinDesk {DateTime.Now.ToString("yyyyy-MM-dd")}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //Setup Http Request
            var client = _httpClientFactory.CreateClient("CoinDesk");
            string url = $"/v1/bpi/currentprice.json";
            var response = await client.GetAsync(url, cancellationToken);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: CoinDesk Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync(cancellationToken);

            //return retrived data in generic format and cache result
            var cachedResult = new DataActorResponse()
            {
                ApiName = "CoinDesk",
                Url = client.BaseAddress + url,
                Body = stringResult

            };

            _memoryCache.Set($"CoinDesk {DateTime.Now.ToString("yyyyy-MM-dd")}", cachedResult);
            return cachedResult;
        }
    }
}
