using ActorApiDI.Domains;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;

namespace ActorApiDI.Clients
{
    public class CoinDeskDataActorClient(IHttpClientFactory _httpClientFactory, IMemoryCache _memoryCache) : IDataActorClient
    {
        /// <summary>
        /// Retrives the Bitcoin Price Index (BPI) in real-time
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetData(DataActorRequest request)
        {
            //Check if cache has this entry and return it
            if (_memoryCache.TryGetValue($"CoinDesk {DateTime.Now.ToString("yyyyy-MM-dd")}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //Setup Http Request
            var client = _httpClientFactory.CreateClient("CoinDesk");
            string url = $"/v1/bpi/currentprice.json";
            var response = await client.GetAsync(url);
            //Success response check
            if (!response.IsSuccessStatusCode) 
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: CoinDesk Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync();

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
