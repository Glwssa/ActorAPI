# ActorAPI

ActorAPI is a .NET Web API that uses dependency injection and provider-based routing to call different external HTTP clients through a single API endpoint.

The project started as a small API routing demo and is currently being modernized into a cleaner backend portfolio project. The current version keeps the original idea but improves the project structure, naming, tests, request contract, and Swagger documentation.

## What the project does

ActorAPI receives a request with a selected client type and provider-specific parameters.

The request is handled by the controller, passed to the `ActorService`, and then routed through the `ActorProviderClientResolver` to the correct provider client.

```text
Controller
  -> ActorService
    -> ActorProviderClientResolver
      -> IActorProviderClient implementation
        -> External API
```

## Main technologies

* .NET 10
* ASP.NET Core Web API
* Dependency Injection
* `HttpClientFactory`
* Swagger / OpenAPI
* XML documentation comments
* User Secrets for local API keys
* xUnit
* Moq

## Project structure

```text
ActorAPI/
  src/
    ActorApi.Api/
      Clients/
      Contracts/
      Controllers/
      Domains/
      Extensions/
      Services/
        Resolvers/
  tests/
    ActorApi.Tests/
      Services/
        Resolvers/
```

## Main endpoint

```http
POST /Actor/Data
```

The endpoint accepts a generic request body:

```json
{
  "clientSelection": "CatFacts",
  "parameters": {},
  "headers": {}
}
```

## Request contract

The API uses a generic dictionary-based request contract.

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

| Field             | Description                                             |
| ----------------- | ------------------------------------------------------- |
| `clientSelection` | Selects which provider client should handle the request |
| `parameters`      | Provider-specific query/request parameters              |
| `headers`         | Provider-specific headers or local credential overrides |

This replaced the older `Param1`, `Param2`, `Header1`, and `Header2` fields with a cleaner dictionary-based structure.

## Current clients

### OpenWeather

Retrieves real-time weather data for a requested city using the OpenWeather API.

Example request:

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

Expected request values:

| Type      | Key      | Description                                         |
| --------- | -------- | --------------------------------------------------- |
| Parameter | `city`   | City name, for example `Athens`                     |
| Parameter | `units`  | Optional unit system, for example `metric`          |
| Header    | `apiKey` | Optional API key override for local Swagger testing |

The API key can also be provided through configuration or User Secrets.

---

### Spotify

Retrieves information about a requested artist using the Spotify API.

Current status: **disabled**

Spotify is currently disabled because its authentication flow needs to be refactored.

Example request shape:

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

Expected request values:

| Type      | Key            | Description               |
| --------- | -------------- | ------------------------- |
| Parameter | `artistId`     | Spotify artist identifier |
| Header    | `clientId`     | Spotify client ID         |
| Header    | `clientSecret` | Spotify client secret     |

---

### News

Retrieves news data from a specific keyword.

Current status: **disabled**

News is currently disabled because the existing provider integration appears to be unavailable or unreliable.

Example request shape:

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

Expected request values:

| Type      | Key        | Description                              |
| --------- | ---------- | ---------------------------------------- |
| Parameter | `query`    | Article keyword or search term           |
| Parameter | `country`  | Optional country code, for example `us`  |
| Parameter | `language` | Optional language code, for example `en` |
| Header    | `apiKey`   | Optional API key override                |

---

### CoinDesk

Retrieves the Bitcoin Price Index in real time.

Example request:

```json
{
  "clientSelection": "CoinDesk",
  "parameters": {},
  "headers": {}
}
```

Expected request values:

| Type       | Required values |
| ---------- | --------------- |
| Parameters | None            |
| Headers    | None            |

---

### CatFacts

Retrieves a random cat fact.

Example request:

```json
{
  "clientSelection": "CatFacts",
  "parameters": {},
  "headers": {}
}
```

Expected request values:

| Type       | Required values |
| ---------- | --------------- |
| Parameters | None            |
| Headers    | None            |

## Configuration

Provider configuration is read from application configuration.

For local development, use .NET User Secrets instead of committing real API keys.

Initialize user secrets:

```bash
dotnet user-secrets init --project src/ActorApi.Api/ActorApi.Api.csproj
```

Set an OpenWeather API key:

```bash
dotnet user-secrets set "Providers:OpenWeather:ApiKey" "YOUR_KEY" --project src/ActorApi.Api/ActorApi.Api.csproj
```

Example configuration shape:

```json
{
  "Providers": {
    "OpenWeather": {
      "ApiKey": ""
    },
    "Spotify": {
      "Enabled": false
    },
    "News": {
      "Enabled": false,
      "ApiKey": ""
    }
  }
}
```

Do not commit real API keys.

## Running the project

Restore packages:

```bash
dotnet restore ActorApi.sln
```

Build the solution:

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

The current tests cover:

* `ActorService` delegation to the resolved provider client
* `ActorProviderClientResolver` provider selection
* Disabled Spotify behavior
* Disabled News behavior
* Unsupported provider behavior

## Current modernization status

Completed modernization work includes:

* Renamed and reorganized the solution structure
* Moved source code under `src/`
* Moved tests under `tests/`
* Updated namespaces
* Upgraded the project to .NET 10
* Replaced keyed provider injection in the service with a provider resolver
* Replaced unclear request fields with dictionary-based `parameters` and `headers`
* Added Swagger/XML documentation
* Added unit tests for service and resolver behavior
* Temporarily disabled providers that require further refactoring

## Planned technical improvements

* Move provider configuration into typed options
* Add centralized exception handling middleware
* Add request validation
* Improve Swagger examples
* Replace generic dictionary requests with provider-specific request contracts later
* Add integration tests
* Add GitHub Actions CI
* Add structured logging
* Add health checks

## To-do clients

The original planned future clients were:

| Client            | Planned purpose                      |
| ----------------- | ------------------------------------ |
| Google Translate  | Translation provider                 |
| GitHub            | GitHub repository/user data provider |
| Twitter / X       | Social media data provider           |
| Covid-19 Database | Covid-19 statistics/data provider    |
| IMDb              | Movie/TV data provider               |

