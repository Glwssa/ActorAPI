using ActorApi.Api.Contracts;
using ActorApi.Api.Extensions;
using ActorApi.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ActorApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ActorController(IActorService actorService, IValidator<DataActorRequest> validator) : ControllerBase
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
        /// Example request for CoinDesk, currently disabled:
        ///
        ///     {
        ///       "clientSelection": "CoinDesk",
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
        /// Example request for News: (Disabled)
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
        /// <param name="cancellationToken"></param>
        /// <returns>The external provider response.</returns>
        [HttpPost("Data")]
        public async Task<ActionResult<DataActorResponse>> GetData([FromBody] DataActorRequest? request, CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                return ValidationBadRequest(
                    new Dictionary<string, string[]>
                    {
                        ["request"] = ["Request body is required."]
                    });
            }

            var validationResult = await validator.ValidateAsync(
                request,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                return ValidationBadRequest(
                    validationResult.ToValidationProblemDictionary());
            }

            var response = await actorService.GetDataAsync(request, cancellationToken);

            return Ok(response);
        }

        private static BadRequestObjectResult ValidationBadRequest(IDictionary<string, string[]> errors)
        {
            return new BadRequestObjectResult(new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Detail = "One or more validation errors occurred."
            });
        }
    }
}
