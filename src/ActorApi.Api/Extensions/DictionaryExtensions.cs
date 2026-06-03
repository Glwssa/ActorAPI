namespace ActorApi.Api.Extensions
{
    public static class DictionaryExtensions
    {
        public static string? GetValueOrDefaultIgnoreCase(
            this IReadOnlyDictionary<string, string> values,
            string key)
        {
            var match = values.FirstOrDefault(
                pair => string.Equals(
                    pair.Key,
                    key,
                    StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrWhiteSpace(match.Key)
                ? null
                : match.Value;
        }
    }
}
