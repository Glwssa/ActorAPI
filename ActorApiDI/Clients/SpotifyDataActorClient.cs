
using ActorApiDI.Domains;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;

namespace ActorApiDI.Clients
{

    public class SpotifyDataActorClient(IHttpClientFactory _httpClientFactory) : IDataActorClient
    {
        /// <summary>
        /// Retrives information about the requested artist 
        /// </summary>
        /// <param name="request">Request Parameters (Param1 = ArtistId, Header1 = ClientID, Header2 = ClientSecret)</param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        /// <exception cref="HttpIOException"></exception>
        public async Task<DataActorResponse> GetData(DataActorRequest request)
        {
            //Valid fields check
            if (request.Param1 is null || request.Header1 is null || request.Header2 is null)
                throw new BadHttpRequestException("Error: Please provide all the required fields. (Param1/Header1/Header2)", 400);
            //Authentication HttpRequest setup
            var authClient = _httpClientFactory.CreateClient("SpotifyAuthClient");
            var AuthRequest = new HttpRequestMessage(new HttpMethod("POST"), "/api/token");
            AuthRequest.Content = new StringContent($"grant_type=client_credentials&client_id={request.Header1}&client_secret={request.Header2}");
            AuthRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var Authresponse = await authClient.SendAsync(AuthRequest);
            //Success authentication response check
            if (!Authresponse.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.UserAuthenticationError, "Error: Spotify Authentication Service was not successfull, please check your cridentials.");
            var authStringResult = await Authresponse.Content.ReadAsStringAsync();
            var auth = JsonConvert.DeserializeObject<SpotifyAuthResponse>(authStringResult);
            
            //Request
            var client = _httpClientFactory.CreateClient("SpotifyClient");
            string url = $"/v1/artists/{request.Param1}";
            var Request = new HttpRequestMessage(new HttpMethod("GET"), url);
            Request.Headers.TryAddWithoutValidation("Authorization", $"{auth!.token_type}  {auth!.access_token}");

            var response = await client.SendAsync(Request);
            //Success response check
            if (!response.IsSuccessStatusCode)
                throw new HttpIOException(HttpRequestError.ConnectionError, "Error: Spotify Service was not available.");
            var stringResult = await response.Content.ReadAsStringAsync();
            //return retrived data in generic format
            return new DataActorResponse()
            {
                ApiName = "Spotify",
                Url = client.BaseAddress + url,
                Body = stringResult

            };

        }
    }
}
