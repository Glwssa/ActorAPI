using ActorApi.Api.Clients;
using ActorApi.Api.Domains;
using ActorApi.Api.Options;
using ActorApi.Api.Services.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace ActorApi.Tests.Services.Resolvers;

public sealed class ActorProviderClientResolverTests
{
    [Fact]
    public void Resolve_WhenProviderExists_ReturnsMatchingClient()
    {
        // Arrange
        var catFactsClientMock = new Mock<IActorProviderClient>();
        catFactsClientMock
            .SetupGet(client => client.ClientSelection)
            .Returns(ClientSelection.CatFacts);

        var weatherClientMock = new Mock<IActorProviderClient>();
        weatherClientMock
            .SetupGet(client => client.ClientSelection)
            .Returns(ClientSelection.OpenWeather);

        var resolver = new ActorProviderClientResolver(
            new[]
            {
            catFactsClientMock.Object,
            weatherClientMock.Object
            },
            CreateSpotifyOptions(enabled: false),
            CreateNewsOptions(enabled: false),
            CreateCoinDeskOptions(enabled: false));

        // Act
        var result = resolver.Resolve(ClientSelection.CatFacts);

        // Assert
        Assert.Same(catFactsClientMock.Object, result);
    }

    [Fact]
    public void Resolve_WhenSpotifyIsDisabled_ThrowsBadHttpRequestException()
    {
        // Arrange
        var spotifyClientMock = new Mock<IActorProviderClient>();
        spotifyClientMock
            .SetupGet(client => client.ClientSelection)
            .Returns(ClientSelection.Spotify);

        var resolver = new ActorProviderClientResolver(
            new[] { spotifyClientMock.Object },
            CreateSpotifyOptions(enabled: false),
            CreateNewsOptions(enabled: true),
            CreateCoinDeskOptions(enabled: false));

        // Act
        var exception = Assert.Throws<BadHttpRequestException>(
            () => resolver.Resolve(ClientSelection.Spotify));

        // Assert
        Assert.Contains("Spotify provider is currently disabled", exception.Message);
        Assert.Equal(400, exception.StatusCode);
    }

    [Fact]
    public void Resolve_WhenProviderDoesNotExist_ThrowsBadHttpRequestException()
    {
        // Arrange
        var catFactsClientMock = new Mock<IActorProviderClient>();
        catFactsClientMock
            .SetupGet(client => client.ClientSelection)
            .Returns(ClientSelection.CatFacts);

        var resolver = new ActorProviderClientResolver(
            new[] { catFactsClientMock.Object },
            CreateSpotifyOptions(enabled: false),
            CreateNewsOptions(enabled: false),
            CreateCoinDeskOptions(enabled: false));

        // Act
        var exception = Assert.Throws<BadHttpRequestException>(
            () => resolver.Resolve(ClientSelection.OpenWeather));

        // Assert
        Assert.Contains("is not supported", exception.Message);
        Assert.Equal(400, exception.StatusCode);
    }

    [Fact]
    public void Resolve_WhenNewsIsDisabled_ThrowsBadHttpRequestException()
    {
        // Arrange
        var newsClientMock = new Mock<IActorProviderClient>();
        newsClientMock
            .SetupGet(client => client.ClientSelection)
            .Returns(ClientSelection.News);

        var resolver = new ActorProviderClientResolver(
            new[] { newsClientMock.Object },
            CreateSpotifyOptions(enabled: false),
            CreateNewsOptions(enabled: false),
            CreateCoinDeskOptions(enabled: false));

        // Act
        var exception = Assert.Throws<BadHttpRequestException>(
            () => resolver.Resolve(ClientSelection.News));

        // Assert
        Assert.Contains("News provider is currently disabled", exception.Message);
        Assert.Equal(400, exception.StatusCode);
    }

    [Fact]
    public void Resolve_WhenCoinDeskIsDisabled_ThrowsBadHttpRequestException()
    {
        // Arrange
        var coinDeskClientMock = new Mock<IActorProviderClient>();
        coinDeskClientMock
            .SetupGet(client => client.ClientSelection)
            .Returns(ClientSelection.CoinDesk);

        var resolver = new ActorProviderClientResolver(
            new[] { coinDeskClientMock.Object },
            CreateSpotifyOptions(enabled: true),
            CreateNewsOptions(enabled: true),
            CreateCoinDeskOptions(enabled: false));

        // Act
        var exception = Assert.Throws<BadHttpRequestException>(
            () => resolver.Resolve(ClientSelection.CoinDesk));

        // Assert
        Assert.Contains("CoinDesk provider is currently disabled", exception.Message);
        Assert.Equal(400, exception.StatusCode);
    }

    private static IOptionsSnapshot<SpotifyOptions> CreateSpotifyOptions(bool enabled)
    {
        var options = new Mock<IOptionsSnapshot<SpotifyOptions>>();

        options
            .SetupGet(x => x.Value)
            .Returns(new SpotifyOptions
            {
                Enabled = enabled
            });

        return options.Object;
    }

    private static IOptionsSnapshot<NewsOptions> CreateNewsOptions(bool enabled)
    {
        var options = new Mock<IOptionsSnapshot<NewsOptions>>();

        options
            .SetupGet(x => x.Value)
            .Returns(new NewsOptions
            {
                Enabled = enabled
            });

        return options.Object;
    }
    private static IOptionsSnapshot<CoinDeskOptions> CreateCoinDeskOptions(bool enabled)
    {
        var options = new Mock<IOptionsSnapshot<CoinDeskOptions>>();

        options
            .SetupGet(x => x.Value)
            .Returns(new CoinDeskOptions
            {
                Enabled = enabled
            });

        return options.Object;
    }
}

