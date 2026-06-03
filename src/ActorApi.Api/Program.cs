using ActorApi.Api.Clients;
using ActorApi.Api.Contracts;
using ActorApi.Api.ExceptionHandling;
using ActorApi.Api.Options;
using ActorApi.Api.Services;
using ActorApi.Api.Services.Resolvers;
using ActorApi.Api.Validators;
using FluentValidation;
using System.Reflection;
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
                builder.Services.Configure<OpenWeatherOptions>(
                    builder.Configuration.GetSection(OpenWeatherOptions.SectionName));

                builder.Services.Configure<CoinDeskOptions>(
                    builder.Configuration.GetSection(CoinDeskOptions.SectionName));

                builder.Services.Configure<CatFactsOptions>(
                    builder.Configuration.GetSection(CatFactsOptions.SectionName));

                builder.Services.Configure<SpotifyOptions>(
                    builder.Configuration.GetSection(SpotifyOptions.SectionName));

                builder.Services.Configure<NewsOptions>(
                    builder.Configuration.GetSection(NewsOptions.SectionName));

                builder.Services.AddHttpClient("OpenWeatherClient", client =>
                {
                    var options = builder.Configuration
                        .GetSection(OpenWeatherOptions.SectionName)
                        .Get<OpenWeatherOptions>() ?? new OpenWeatherOptions();

                    client.BaseAddress = new Uri(options.BaseUrl);
                });
                builder.Services.AddHttpClient("CoinDesk", client =>
                {
                    var options = builder.Configuration
                        .GetSection(CoinDeskOptions.SectionName)
                        .Get<CoinDeskOptions>() ?? new CoinDeskOptions();

                    client.BaseAddress = new Uri(options.BaseUrl);
                });
                builder.Services.AddHttpClient("SpotifyClient", client =>
                {
                    var options = builder.Configuration
                        .GetSection(SpotifyOptions.SectionName)
                        .Get<SpotifyOptions>() ?? new SpotifyOptions();

                    client.BaseAddress = new Uri(options.BaseUrl);
                });

                builder.Services.AddHttpClient("SpotifyAuthClient", client =>
                {
                    var options = builder.Configuration
                        .GetSection(SpotifyOptions.SectionName)
                        .Get<SpotifyOptions>() ?? new SpotifyOptions();

                    client.BaseAddress = new Uri(options.AuthBaseUrl);
                });
                builder.Services.AddHttpClient("NewsClient", client =>
                {
                    var options = builder.Configuration
                        .GetSection(NewsOptions.SectionName)
                        .Get<NewsOptions>() ?? new NewsOptions();

                    client.BaseAddress = new Uri(options.BaseUrl);
                });
                builder.Services.AddHttpClient("CatFacts", client =>
                {
                    var options = builder.Configuration
                        .GetSection(CatFactsOptions.SectionName)
                        .Get<CatFactsOptions>() ?? new CatFactsOptions();

                    client.BaseAddress = new Uri(options.BaseUrl);
                });
                //Setup Actor Service (for every request a new ActorService is created)
                builder.Services.AddScoped<IActorService, ActorService>();
                builder.Services.AddScoped<IActorProviderClientResolver, ActorProviderClientResolver>();
                //Setup Actor Clients with Keys for easy search from Actorservice (for every request a new ActorClient is created)
                builder.Services.AddScoped<IActorProviderClient, OpenWeatherProviderClient>();
                builder.Services.AddScoped<IActorProviderClient, CatFactsProviderClient>();
                builder.Services.AddScoped<IActorProviderClient, NewsProviderClient>();
                builder.Services.AddScoped<IActorProviderClient, SpotifyProviderClient>();
                builder.Services.AddScoped<IActorProviderClient, CoinDeskProviderClient>();
                //Setup FluentValidation for DataActorRequest (for every request a new DataActorRequestValidator is created)
                builder.Services.AddScoped<IValidator<DataActorRequest>, DataActorRequestValidator>();

                //Add Memory caching 
                builder.Services.AddMemoryCache();
                //Add Controllers with correct display names for Enums
                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

                    if (File.Exists(xmlFilePath))
                    {
                        options.IncludeXmlComments(xmlFilePath);
                    }
                });
            }
            //Add global exception handling and problem details services
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();
            {
                app.UseExceptionHandler();

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



