using ActorApi.Api.Domains;
using ActorApi.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActorApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorController(IActorService _dataActorService) : Controller
    {
        /// <summary>
        /// Gets Data depending the Client requested.
        /// </summary>
        /// <param name="request">Parameters for Client Request</param>
        /// <returns></returns>
        [HttpGet("GetData")]
        public async Task<DataActorResponse> GetData([FromQuery]DataActorRequest request)
        {
            return await _dataActorService.GetData(request);
        }
    }
}
