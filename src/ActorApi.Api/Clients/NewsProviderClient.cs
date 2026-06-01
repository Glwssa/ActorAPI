using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace ActorApi.Api.Clients
{
    public class NewsProviderClient(IHttpClientFactory _httpClientFactory, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.News;

        /// <summary>
        /// Retrives News data from a specific keyword that was published today.
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = Article Keyword, Header1 = ApiKey)</param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request)
        {
            //Valid fields check
            var article = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.Article);
            var key = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ApiKey);
            if (article is null || key is null)
                throw new BadHttpRequestException("Error: Please provide all the required fields. (Param1/Header1)", 400);
            DateTime dateToday = DateTime.Now;
            string url = $"/v2/everything?q={article}&from={dateToday.ToString("yyyy-MM-dd")}&sortBy=publishedAt&apiKey={key}";
            //caching check
            if (_memoryCache.TryGetValue($"News {url}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //Setup Http Request with specific day
            var client = _httpClientFactory.CreateClient("NewsClient");
            var Request = new HttpRequestMessage(new HttpMethod("GET"), url);
            var response = await client.SendAsync(Request);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: News Service was not available.");
            var stringResult = await response.Content.ReadAsStringAsync();

            //return retrived data in generic format and add it to cache
            var cachedResult = new DataActorResponse()
            {
                ApiName = "News",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
            _memoryCache.Set($"News {url}", cachedResult);

            return cachedResult;
        }
    }
}
