using ActorApiDI.Domains;
using System;
using System.Net.Http;

namespace ActorApiDI.Clients
{
    public class CoinDeskDataActorClient(IHttpClientFactory _httpClientFactory) : IDataActorClient
    {
        /// <summary>
        /// Retrives the Bitcoin Price Index (BPI) in real-time
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetData(DataActorRequest request)
        {
            //Setup Http Request
            var client = _httpClientFactory.CreateClient("CoinDesk");
            string url = $"/v1/bpi/currentprice.json";
            var response = await client.GetAsync(url);
            //Success response check
            if (!response.IsSuccessStatusCode) 
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: CoinDesk Service was not available.");

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
