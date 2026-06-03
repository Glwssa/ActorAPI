# ActorAPI

ActorAPI is a .NET 10 Web API that routes generic provider requests to external HTTP clients through a provider resolver.

The project exposes a single request model that contains a selected provider, a dictionary of provider-specific parameters, and a dictionary of optional header or credential overrides. The request is validated, routed to the correct provider client, executed through `HttpClientFactory`, and returned as a generic response.

## Features

* ASP.NET Core Web API
* .NET 10
* Provider-based request routing
* `HttpClientFactory`
* Typed provider options
* Manual FluentValidation request validation
* Centralized exception handling
* ProblemDetails error responses
* In-memory caching
* Swagger/OpenAPI documentation
* XML documentation comments
* xUnit and Moq tests
* User Secrets support for local credentials

## Architecture

```text
Controller
  -> ActorService
    -> ActorProviderClientResolver
      -> IActorProviderClient implementation
        -> External API
```

### Main flow

1. `ActorController` receives a `DataActorRequest`.
2. `DataActorRequestValidator` validates the request.
3. `ActorService` delegates provider selection to `ActorProviderClientResolver`.
4. `ActorProviderClientResolver` selects the correct `IActorProviderClient`.
5. The selected provider client calls the external API.
6. The API returns a `DataActorResponse`.
7. Errors are handled through centralized exception handling and returned as ProblemDetails responses.

## Project structure

```text
ActorAPI/
  src/
    ActorApi.Api/
      Clients/
      Contracts/
      Controllers/
      Domain/
      ExceptionHandling/
      Exceptions/
      Extensions/
      Options/
      Services/
        Resolvers/
      Validators/

  tests/
    ActorApi.Tests/
      Extensions/
      Services/
        Resolvers/
      Validators/
```

## Main endpoint

```http
POST /Actor/Data
```

## Request contract

The API uses a generic dictionary-based request model.

```json
{
  "clientSelection": "ProviderName",
  "parameters": {
    "key": "value"
  },
  "headers": {
    "key": "value"
  }
}
```

### Fields

| Field             | Description                                               |
| ----------------- | --------------------------------------------------------- |
| `clientSelection` | The provider that should handle the request               |
| `parameters`      | Provider-specific request/query values                    |
| `headers`         | Optional provider-specific header or credential overrides |

## Response contract

The API returns a generic response shape.

```json
{
  "apiName": "CatFacts",
  "url": "https://catfact.ninja/fact",
  "body": "{ ... }"
}
```

### Fields

| Field     | Description                                         |
| --------- | --------------------------------------------------- |
| `apiName` | Name of the provider that handled the request       |
| `url`     | External URL that was called                        |
| `body`    | Raw response body returned by the external provider |

## Current providers

| Provider    | Status   | Notes                                                                   |
| ----------- | -------- | ----------------------------------------------------------------------- |
| CatFacts    | Enabled  | Returns a random cat fact                                               |
| OpenWeather | Enabled  | Requires `city`; API key can come from User Secrets or `headers.apiKey` |
| Spotify     | Disabled | Authentication flow needs refactoring                                   |
| News        | Disabled | Current endpoint is unavailable or unreliable                                  |
| CoinDesk    | Disabled | Current endpoint is unavailable or unreliable                           |

## Provider examples

### CatFacts

CatFacts does not require parameters or headers.

```json
{
  "clientSelection": "CatFacts",
  "parameters": {},
  "headers": {}
}
```

### OpenWeather

OpenWeather requires a city.

```json
{
  "clientSelection": "OpenWeather",
  "parameters": {
    "city": "Athens",
    "units": "metric"
  },
  "headers": {
    "apiKey": "optional-local-api-key"
  }
}
```

If `headers.apiKey` is not provided, the application attempts to read the API key from configuration/User Secrets.

### Spotify

Spotify is currently disabled.

```json
{
  "clientSelection": "Spotify",
  "parameters": {
    "artistId": "example-artist-id"
  },
  "headers": {
    "clientId": "example-client-id",
    "clientSecret": "example-client-secret"
  }
}
```

### News

News is currently disabled.

```json
{
  "clientSelection": "News",
  "parameters": {
    "query": "technology",
    "country": "us",
    "language": "en"
  },
  "headers": {
    "apiKey": "optional-local-api-key"
  }
}
```

### CoinDesk

CoinDesk is currently disabled.

```json
{
  "clientSelection": "CoinDesk",
  "parameters": {},
  "headers": {}
}
```

## Known request keys

### Parameter keys

| Key        | Used by     | Description                                        |
| ---------- | ----------- | -------------------------------------------------- |
| `city`     | OpenWeather | City name, for example `Athens`                    |
| `units`    | OpenWeather | Unit system, for example `metric`                  |
| `query`    | News        | Search query                                       |
| `country`  | News        | Country code, for example `us`                     |
| `language` | News        | Language code, for example `en`                    |
| `article`  | News        | Article identifier, if supported by provider logic |
| `artistId` | Spotify     | Spotify artist identifier                          |

### Header keys

| Key             | Used by           | Description                           |
| --------------- | ----------------- | ------------------------------------- |
| `apiKey`        | OpenWeather, News | Optional per-request API key override |
| `authorization` | Future providers  | Optional authorization token          |
| `clientId`      | Spotify           | Spotify client ID                     |
| `clientSecret`  | Spotify           | Spotify client secret                 |

## Configuration

Provider settings are read from application configuration.

Non-secret configuration, such as base URLs and enabled flags, belongs in `appsettings.json`.

Real credentials should be stored with .NET User Secrets or environment variables.

### Example `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Providers": {
    "OpenWeather": {
      "BaseUrl": "http://api.openweathermap.org"
    },
    "CoinDesk": {
      "Enabled": false,
      "BaseUrl": "https://api.coindesk.com"
    },
    "CatFacts": {
      "BaseUrl": "https://catfact.ninja"
    },
    "Spotify": {
      "Enabled": false,
      "BaseUrl": "https://api.spotify.com",
      "AuthBaseUrl": "https://accounts.spotify.com"
    },
    "News": {
      "Enabled": false,
      "BaseUrl": "https://newsapi.org"
    }
  }
}
```

## Validation

Requests are validated manually in `ActorController` using FluentValidation.

Current validation rules:

| Rule               | Description                                      |
| ------------------ | ------------------------------------------------ |
| `ClientSelection`  | Must be a valid enum value                       |
| `Parameters`       | Cannot be null                                   |
| `Headers`          | Cannot be null                                   |
| OpenWeather `city` | Required when `clientSelection` is `OpenWeather` |

Invalid requests return a validation ProblemDetails response.

Example invalid OpenWeather request:

```json
{
  "clientSelection": "OpenWeather",
  "parameters": {},
  "headers": {}
}
```

Expected result:

```text
400 Bad Request
```

with a validation error explaining that OpenWeather requires the `city` parameter.

## Error handling

The API uses centralized exception handling.

Custom exceptions derive from `ActorApiException` and are converted into ProblemDetails responses.

Current custom exceptions:

| Exception                        | Purpose                                                                  |
| -------------------------------- | ------------------------------------------------------------------------ |
| `ProviderDisabledException`      | Returned when a known provider is temporarily disabled                   |
| `ProviderNotSupportedException`  | Returned when no registered provider client exists                       |
| `ProviderUnavailableException`   | Returned when an external provider fails or is unavailable               |
| `ProviderConfigurationException` | Returned when provider configuration or required credentials are missing |

Example disabled-provider response:

```json
{
  "title": "Provider disabled",
  "status": 400,
  "detail": "Spotify provider is currently disabled because it requires authentication refactoring.",
  "instance": "/Actor/Data",
  "traceId": "..."
}
```

## Caching

Some provider clients use in-memory caching through `IMemoryCache`.

Cache keys are generated through `CacheKeyExtensions`.

Examples:

```text
CatFacts:2026-06-03
OpenWeather:athens:metric
```

Caching is used only where it makes sense for the current provider behavior.

## Running locally

Restore packages:

```bash
dotnet restore ActorApi.sln
```

Build:

```bash
dotnet build ActorApi.sln
```

Run the API:

```bash
dotnet run --project src/ActorApi.Api/ActorApi.Api.csproj
```

Open Swagger:

```text
/swagger
```

## Running tests

```bash
dotnet test ActorApi.sln
```

The test suite currently covers:

* `ActorService` delegation to the resolved provider client
* `ActorProviderClientResolver` provider selection
* Disabled provider behavior
* Unsupported provider behavior
* Request validation rules
* Cache key generation

## Planned clients

| Client                   | Status                       | Notes                                                            |
| ------------------------ | ---------------------------- | ---------------------------------------------------------------- |
| Open-Meteo               | Planned                      | Weather provider without API key requirement                     |
| GitHub                   | Planned                      | Repository/user data provider                                    |
| OMDb / movie data        | Planned                      | Replacement path for the original IMDb idea                      |
| Public health statistics | Planned                      | Broader replacement path for the original Covid-19 database idea |
| Spotify                  | Planned refactor             | Authentication flow needs cleanup                                |
| News                     | Planned replacement/refactor | Current integration is unreliable                                |

## Deferred original client ideas

These clients were part of the original idea but are not planned for the next implementation stage.

| Client            | Reason deferred                                             |
| ----------------- | ----------------------------------------------------------- |
| Google Translate  | Cloud setup and cost complexity                             |
| Twitter / X       | API cost and authentication complexity                      |
| IMDb              | Will likely be represented through OMDb instead             |
| Covid-19 Database | Will likely become broader public-health statistics instead |

## Development notes

* Keep real credentials out of source control.
* Prefer User Secrets for local development credentials.
* Keep provider-specific behavior inside provider clients.
* Keep request routing inside `ActorProviderClientResolver`.
* Keep validation rules inside validators.
* Keep HTTP error translation inside centralized exception handling.
