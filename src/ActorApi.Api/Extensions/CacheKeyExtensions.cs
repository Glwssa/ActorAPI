namespace ActorApi.Api.Extensions;

public static class CacheKeyExtensions
{
    public static string CreateProviderCacheKey(
        string providerName,
        string value)
    {
        return $"{providerName}:{value}";
    }

    public static string CreateDailyProviderCacheKey(
        string providerName,
        DateOnly date)
    {
        return $"{providerName}:{date:yyyy-MM-dd}";
    }
}