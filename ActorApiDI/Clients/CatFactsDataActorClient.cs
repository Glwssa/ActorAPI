
using ActorApiDI.Domains;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace ActorApiDI.Clients
{
    public class CatFactsDataActorClient(IHttpClientFactory _httpClientFactory) : IDataActorClient
    {
        /// <summary>
        /// Retrives random cat facts
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetData(DataActorRequest request)
        {
            //Setup HttpRequest
            var client = _httpClientFactory.CreateClient("CatFacts");
            string url = $"/fact";
            var response = await client.GetAsync(url);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: CatFacts Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync();

            //return retrived data in generic format
            return new DataActorResponse()
            {
                ApiName = "CoinDesk",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
        }
    }
}
