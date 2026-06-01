using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace ActorApi.Api.Clients
{
    public class OpenWeatherProviderClient(IHttpClientFactory _httpClientFactory, IMemoryCache _memoryCache) : IActorProviderClient
    {
        public ClientSelection ClientSelection => ClientSelection.OpenWeather;

        /// <summary>
        /// Retrives weather data for the requested city.
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = city, Header 1 = ApiKey)</param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetDataAsync(DataActorRequest request)
        {
            //Valid fields check
            var city = request.Parameters.GetValueOrDefaultIgnoreCase(RequestParameterKeys.City);
            var apiKey = request.Headers.GetValueOrDefaultIgnoreCase(RequestHeaderKeys.ApiKey);
            if (city is null || apiKey is null)
                throw new BadHttpRequestException("Error: Please provide all the required fields. (Param1/Header1)", 400);
            //caching check
            string url = $"/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
            if (_memoryCache.TryGetValue($"OpenWeather {url}", out DataActorResponse? result) && result is not null)
            {
                return result;
            }

            //Setup HttpRequest
            var client = _httpClientFactory.CreateClient("OpenWeatherClient");
            var response = await client.GetAsync(url);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: OpenWeather Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync();

            //return retrived data in generic format and cache it
            var cachedResult = new DataActorResponse()
            {
                ApiName = "OpenWeather",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
            _memoryCache.Set($"OpenWeather {url}", cachedResult);

            return cachedResult;
        }
    }
}
