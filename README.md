# ActorAPI

A .NET Web API utilizes Dependency Injection to dynamically route requests to various HTTP clients. By simply invoking the GetData endpoint with the appropriate ClientSelection and RequestData parameters, the ActorService selects and engages the designated HTTP client to execute the specified GetData method. 

List of Clients:
  - OpenWeather:
      Retrives real time weather data for the requested city using the OpenWeather API.
    
    Data Required for Request:
      --Param1 = City Name.
      --Header1 = ApiKey
  - Spotify:
      Retrives information abouta a requested Artist using the Spotify API.
    
    Data Required for Request:
      --Param1 = Artist ID.
      --Header1 = Client ID
      --Header2 = Client Secret
  - News:
      Retrives News data from a specific keyword that was published today using the newsapi.
    
    Data Required for Request:
      --Param1 = Article Keyword.
      --Header1 = ApiKey
  - CoinDesk:
      Retrives the Bitcoin Price Index (BPI) in real-time.
    
    Data Required for Request:
      None
  - CateFacts:
      Retrives random cat facts
    
    Data Required for Request:
      None

NOTE: Example Params and Headers for each Client are located in appsetting.Development.json

List of To-Do Clients:
  - Google Translate
  - GitHub
  - Twitter (now X)
  - Covid 19 Database
  - Imdb
