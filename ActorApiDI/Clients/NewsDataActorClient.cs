
using ActorApiDI.Domains;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace ActorApiDI.Clients
{
    public class NewsDataActorClient(IHttpClientFactory _httpClientFactory) : IDataActorClient
    {
        /// <summary>
        /// Retrives News data from a specific keyword that was published today.
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = Article Keyword, Header1 = ApiKey)</param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetData(DataActorRequest request)
        {
            //Valid fields check
            if (request.Param1 is null || request.Header1 is null)
                throw new BadHttpRequestException("Error: Please provide all the required fields. (Param1/Header1)", 400);
            //Setup Http Request with specific day
            DateTime dateToday = DateTime.Now;
            var client = _httpClientFactory.CreateClient("NewsClient");
            string url = $"/v2/everything?q={request.Param1}&from={dateToday.ToString("yyyy-MM-dd")}&sortBy=publishedAt&apiKey={request.Header1}";
            var Request = new HttpRequestMessage(new HttpMethod("GET"), url);
            var response = await client.SendAsync(Request);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: News Service was not available.");
            var stringResult = await response.Content.ReadAsStringAsync();

            //return retrived data in generic format
            return new DataActorResponse()
            {
                ApiName = "News",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
        }
    }
}
