using ActorApi.Api.Clients;
using ActorApi.Api.Domains;
using ActorApi.Api.Services;
using ActorApi.Api.Services.Resolvers;
using Moq;

namespace ActorApi.Tests.Services;

public sealed class ActorServiceTests
{
    [Fact]
    public async Task GetDataAsync_WhenProviderExists_CallsResolvedProviderClient()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.CatFacts
        };

        var expectedResponse = new DataActorResponse
        {
            ApiName = "CatFacts",
            Url = "https://catfact.ninja/fact",
            Body = "{}"
        };

        var clientMock = new Mock<IActorProviderClient>();
        clientMock
            .Setup(client => client.GetDataAsync(request))
            .ReturnsAsync(expectedResponse);

        var resolverMock = new Mock<IActorProviderClientResolver>();
        resolverMock
            .Setup(resolver => resolver.Resolve(ClientSelection.CatFacts))
            .Returns(clientMock.Object);

        var service = new ActorService(resolverMock.Object);

        // Act
        var result = await service.GetDataAsync(request);

        // Assert
        Assert.Equal(expectedResponse, result);

        resolverMock.Verify(
            resolver => resolver.Resolve(ClientSelection.CatFacts),
            Times.Once);

        clientMock.Verify(
            client => client.GetDataAsync(request),
            Times.Once);
    }
}
