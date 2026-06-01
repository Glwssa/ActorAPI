using ActorApi.Api.Clients;
using ActorApi.Api.Contracts;
using ActorApi.Api.Services.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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

        var configuration = CreateConfiguration(spotifyEnabled: false);

        var resolver = new ActorProviderClientResolver(
            new[]
            {
                catFactsClientMock.Object,
                weatherClientMock.Object
            },
            configuration);

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

        var configuration = CreateConfiguration(spotifyEnabled: false);

        var resolver = new ActorProviderClientResolver(
            new[] { spotifyClientMock.Object },
            configuration);

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

        var configuration = CreateConfiguration(spotifyEnabled: false);

        var resolver = new ActorProviderClientResolver(
            new[] { catFactsClientMock.Object },
            configuration);

        // Act
        var exception = Assert.Throws<BadHttpRequestException>(
            () => resolver.Resolve(ClientSelection.CoinDesk));

        // Assert
        Assert.Contains("is not supported", exception.Message);
        Assert.Equal(400, exception.StatusCode);
    }

    private static IConfiguration CreateConfiguration(bool spotifyEnabled)
    {
        var values = new Dictionary<string, string?>
        {
            ["Providers:Spotify:Enabled"] = spotifyEnabled.ToString()
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
