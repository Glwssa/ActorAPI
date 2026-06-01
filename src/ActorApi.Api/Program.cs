
using ActorApi.Api.Clients;
using ActorApi.Api.Services;
using System.Text.Json.Serialization;

namespace ActorApi.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            {
                // Add services to the container.

                //Setup HttpClientFactory services
                builder.Services.AddHttpClient("OpenWeatherClient",
                    client =>
                    {
                        client.BaseAddress = new Uri("http://api.openweathermap.org");
                    });
                builder.Services.AddHttpClient("CoinDesk",
                    client =>
                    {
                        client.BaseAddress = new Uri("https://api.coindesk.com");
                    });
                builder.Services.AddHttpClient("SpotifyClient",
                    client =>
                    {
                        client.BaseAddress = new Uri("https://api.spotify.com");
                    });
                builder.Services.AddHttpClient("SpotifyAuthClient",
                    client =>
                    {
                        client.BaseAddress = new Uri("https://accounts.spotify.com");
                    });
                builder.Services.AddHttpClient("NewsClient",
                    client =>
                    {
                        client.BaseAddress = new Uri("https://newsapi.org");
                    });
                builder.Services.AddHttpClient("CatFacts",
                    client =>
                    {
                        client.BaseAddress = new Uri("https://catfact.ninja");
                    });
                //Setup Actor Service (for every request a new ActorService is created)
                builder.Services.AddScoped<IActorService, ActorService>();
                //Setup Actor Clients with Keys for easy search from Actorservice (for every request a new ActorClient is created)
                builder.Services.AddKeyedScoped<IActorProviderClient, CatFactsProviderClient>("CatFacts");
                builder.Services.AddKeyedScoped<IActorProviderClient, OpenWeatherProviderClient>("Weather");
                builder.Services.AddKeyedScoped<IActorProviderClient, NewsProviderClient>("News");
                builder.Services.AddKeyedScoped<IActorProviderClient, SpotifyProviderClient>("Spotify");
                builder.Services.AddKeyedScoped<IActorProviderClient, CoinDeskProviderClient>("CoinDesk");
                //Add Memory caching 
                builder.Services.AddMemoryCache();
                //Add Controllers with correct display names for Enums
                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

            }


            var app = builder.Build();
            {
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();

            }

        }
    }
}
