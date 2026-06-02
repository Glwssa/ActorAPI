using ActorApi.Api.Contracts;
using ActorApi.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActorApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorController(IActorService _actorService) : Controller
    {
        /// <summary>
        /// Calls the selected external provider client and returns the provider response.
        /// </summary>
        /// <remarks>
        /// Example request for CatFacts:
        ///
        ///     {
        ///       "clientSelection": "CatFacts",
        ///       "parameters": {},
        ///       "headers": {}
        ///     }
        ///
        /// Example request for OpenWeather:
        ///
        ///     {
        ///       "clientSelection": "OpenWeather",
        ///       "parameters": {
        ///         "city": "Athens",
        ///         "units": "metric"
        ///       },
        ///       "headers": {
        ///         "apiKey": "optional-local-override"
        ///       }
        ///     }
        ///
        /// Example request for News:
        ///
        ///     {
        ///       "clientSelection": "News",
        ///       "parameters": {
        ///         "query": "technology",
        ///         "country": "us",
        ///         "language": "en"
        ///       },
        ///       "headers": {
        ///         "apiKey": "optional-local-override"
        ///       }
        ///     }
        ///
        /// Spotify is currently disabled.
        /// </remarks>
        /// <param name="request">The provider request.</param>
        /// <returns>The external provider response.</returns>
        [HttpPost("Data")]
        public async Task<ActionResult<DataActorResponse>> GetData([FromBody] DataActorRequest request)
        {
            var response = await _actorService.GetDataAsync(request);

            return Ok(response);
        }
    }
}
