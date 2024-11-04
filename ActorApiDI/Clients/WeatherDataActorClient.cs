
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ActorApiDI.Domains;

namespace ActorApiDI.Clients
{
    public class WeatherDataActorClient(IHttpClientFactory _httpClientFactory) : IDataActorClient
    {
        /// <summary>
        /// Retrives weather data for the requested city.
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = city, Header 1 = ApiKey)</param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetData(DataActorRequest request)
        {
            //Valid fields check
            if (request.Param1 is null || request.Header1 is null)
                throw new BadHttpRequestException("Error: Please provide all the required fields. (Param1/Header1)", 400);
            //Setup HttpRequest
            var client = _httpClientFactory.CreateClient("OpenWeatherClient");
            string url = $"/data/2.5/weather?q={request.Param1}&appid={request.Header1}&units=metric";
            var response = await client.GetAsync(url);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: OpenWeather Service was not available.");

            var stringResult = await response.Content.ReadAsStringAsync();

            //return retrived data in generic format
            return new DataActorResponse()
            {
                ApiName = "OpenWeather",
                Url = client.BaseAddress + url,
                Body = stringResult

            };
        }
    }
}
