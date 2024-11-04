using ActorApiDI.Domains;
using ActorApiDI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActorApiDI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorController(IDataActorService _dataActorService) : Controller
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
