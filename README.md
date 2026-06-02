# ActorAPI

ActorAPI is a modernized .NET backend API that routes generic provider requests to external API clients through a provider resolver.

The project demonstrates:

- ASP.NET Core Web API
- Dependency Injection
- HttpClientFactory
- Provider-based client routing
- Swagger/OpenAPI documentation
- XML documentation comments
- User Secrets support
- Unit tests with xUnit and Moq
- .NET 10

## Current status

This is a modernization/refactor branch of an older demo project.

Currently supported providers:

| Provider | Status | Required parameters | Required headers |
|---|---|---|---|
| CatFacts | Enabled | None | None |
| CoinDesk | Enabled | None | None |
| OpenWeather | Enabled | `city`, optional `units` | Optional `apiKey` override |
| Spotify | Disabled | `artistId` | `clientId`, `clientSecret` |
| News | Disabled | `query`, `country`, `language` | Optional `apiKey` override |

Spotify is disabled because authentication needs to be refactored.
News is disabled because the current provider integration is no longer reliable.

## Request format

Endpoint:

```http
POST /Actor/Data

List of To-Do Clients:
  - Google Translate
  - GitHub
  - Twitter (now X)
  - Covid 19 Database
  - Imdb
