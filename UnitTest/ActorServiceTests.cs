using ActorApiDI.Clients;
using ActorApiDI.Controllers;
using ActorApiDI.Domains;
using ActorApiDI.Services;
using Moq;

namespace UnitTest
{
    /// <summary>
    /// This XUnit test is not working correctly for now 
    /// </summary>
    public class ActorServiceTests
    {
        private readonly DataActorService _sut;
        private readonly Mock<WeatherDataActorClient> _weatherClientMock = new Mock<WeatherDataActorClient>();
        private readonly Mock<CoinDeskDataActorClient> _coinDeskClientMock = new Mock<CoinDeskDataActorClient>();
        private readonly Mock<SpotifyDataActorClient> _SpotifyClientMock = new Mock<SpotifyDataActorClient>();
        private readonly Mock<NewsDataActorClient> _newsClientMock = new Mock<NewsDataActorClient>();
        private readonly Mock<CatFactsDataActorClient> _gitHubClientMock = new Mock<CatFactsDataActorClient>();
        public ActorServiceTests()
        {
            _sut = new DataActorService(_weatherClientMock.Object,
                _coinDeskClientMock.Object,
                _SpotifyClientMock.Object,
                _newsClientMock.Object,
                _gitHubClientMock.Object);
        }

        [Fact]
        public async Task ActorTest1()
        {
            
            DataActorRequest testRequest = new DataActorRequest()
            {
                clientSelection = ClientSelection.OpenWeather,
                Param1 = "Athens",
                Param2 = null,
                Header1 = "fe5bd2c7fdaad972b2e3079ca79ab40a",
                Header2 = null
            };
            DataActorResponse testResponse = new DataActorResponse()
            {
                ApiName = "OpenWeather",
                Url = "",
                Body = ""
            };

            _weatherClientMock.Setup(x => x.GetData(testRequest))
                .ReturnsAsync(testResponse);

            var result = await _sut.GetData(testRequest);

            Assert.Equal(testResponse.ApiName, result.ApiName);
        }
    }
}