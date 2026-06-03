using ActorApi.Api.Extensions;

namespace ActorApi.Tests.Extensions;

public sealed class CacheKeyExtensionsTests
{
    [Fact]
    public void CreateProviderCacheKey_ReturnsExpectedFormat()
    {
        // Act
        var result = CacheKeyExtensions.CreateProviderCacheKey(
            "OpenWeather",
            "athens:metric");

        // Assert
        Assert.Equal("OpenWeather:athens:metric", result);
    }

    [Fact]
    public void CreateDailyProviderCacheKey_ReturnsExpectedFormat()
    {
        // Arrange
        var date = new DateOnly(2026, 6, 3);

        // Act
        var result = CacheKeyExtensions.CreateDailyProviderCacheKey(
            "CatFacts",
            date);

        // Assert
        Assert.Equal("CatFacts:2026-06-03", result);
    }
}