
using ActorApiDI.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;

namespace ActorApiDI.Clients
{
    public class CatFactsDataActorClient(IHttpClientFactory _httpClientFactory, IMemoryCache _memoryCache) : IDataActorClient
    {
        /// <summary>
        /// Retrives random cat facts
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetData(DataActorRequest request)
        {
            //Caching check
            if (_memoryCache.TryGetValue($"CatFacts {DateTime.Now.ToString("yyyyy-MM-dd")}", out DataActorResponse result) && result is not null)
            {
                return result;
            }

            //Setup HttpRequest
            var client = _httpClientFactory.CreateClient("CatFacts");
            string url = $"/fact";
            var response = await client.GetAsync(url);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: CatFacts Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync();

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
