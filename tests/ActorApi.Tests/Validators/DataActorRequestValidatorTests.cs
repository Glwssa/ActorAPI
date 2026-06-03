using ActorApi.Api.Contracts;
using ActorApi.Api.Domains;
using ActorApi.Api.Validators;

namespace ActorApi.Tests.Validators;

public sealed class DataActorRequestValidatorTests
{
    private readonly DataActorRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WhenCatFactsRequestIsValid_ReturnsValid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.CatFacts,
            Parameters = [],
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WhenOpenWeatherCityIsMissing_ReturnsInvalid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.OpenWeather,
            Parameters = [],
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "OpenWeather requires parameter 'city'.");
    }

    [Fact]
    public async Task ValidateAsync_WhenOpenWeatherCityIsProvided_ReturnsValid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.OpenWeather,
            Parameters =
            {
                [RequestParameterKeys.City] = "Athens"
            },
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WhenOpenWeatherCityKeyUsesDifferentCasing_ReturnsValid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.OpenWeather,
            Parameters =
            {
                ["City"] = "Athens"
            },
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WhenSpotifyArtistIdIsProvided_ReturnsValid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.Spotify,
            Parameters =
            {
                [RequestParameterKeys.ArtistId] = "4Z8W4fKeB5YxbusRsdQVPb"
            },
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WhenSpotifyArtistIdKeyUsesDifferentCasing_ReturnsValid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.Spotify,
            Parameters =
            {
                ["ArtistId"] = "4Z8W4fKeB5YxbusRsdQVPb"
            },
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WhenSpotifyArtistIdIsMissing_ReturnsInvalid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.Spotify,
            Parameters = [],
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "Spotify requires parameter 'ArtistId'.");
    }

    [Fact]
    public async Task ValidateAsync_WhenParametersIsNull_ReturnsInvalid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.CatFacts,
            Parameters = null!,
            Headers = []
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "Parameters dictionary is required.");
    }

    [Fact]
    public async Task ValidateAsync_WhenHeadersIsNull_ReturnsInvalid()
    {
        // Arrange
        var request = new DataActorRequest
        {
            ClientSelection = ClientSelection.CatFacts,
            Parameters = [],
            Headers = null!
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "Headers dictionary is required.");
    }
}