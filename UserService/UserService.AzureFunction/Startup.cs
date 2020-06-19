using HelpMyStreet.Utils.CoordinatedResetCache;
using HelpMyStreet.Utils.PollyPolicies;
using HelpMyStreet.Utils.Utils;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using HelpMyStreet.Cache;
using HelpMyStreet.Cache.Extensions;
using UserService.Core;
using UserService.Core.BusinessLogic;
using UserService.Core.Cache;
using UserService.Core.Config;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Interfaces.Utils;
using UserService.Core.Services;
using UserService.Core.Utils;
using UserService.Handlers;
using UserService.Repo;


[assembly: FunctionsStartup(typeof(UserService.AzureFunction.Startup))]
namespace UserService.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // We need to get the app directory this way.  Using Environment.CurrentDirectory doesn't work in Azure.
            ExecutionContextOptions executioncontextoptions = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            string currentDirectory = executioncontextoptions.AppDirectory;

            var config = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables().Build();

            // DI doesn't work in startup
            PollyHttpPolicies pollyHttpPolicies = new PollyHttpPolicies(new PollyHttpPoliciesConfig());

            Dictionary<HttpClientConfigName, ApiConfig> httpClientConfigs = config.GetSection("Apis").Get<Dictionary<HttpClientConfigName, ApiConfig>>();

            foreach (KeyValuePair<HttpClientConfigName, ApiConfig> httpClientConfig in httpClientConfigs)
            {
                IAsyncPolicy<HttpResponseMessage> retryPolicy = httpClientConfig.Value.IsExternal ? pollyHttpPolicies.ExternalHttpRetryPolicy : pollyHttpPolicies.InternalHttpRetryPolicy;

                builder.Services.AddHttpClient(httpClientConfig.Key.ToString(), c =>
                {
                    c.BaseAddress = new Uri(httpClientConfig.Value.BaseAddress);

                    c.Timeout = httpClientConfig.Value.Timeout ?? new TimeSpan(0, 0, 0, 15);

                    foreach (KeyValuePair<string, string> header in httpClientConfig.Value.Headers)
                    {
                        c.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    c.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    c.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    MaxConnectionsPerServer = httpClientConfig.Value.MaxConnectionsPerServer ?? int.MaxValue,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                }).AddPolicyHandler(retryPolicy);

            }

            builder.Services.AddMediatR(typeof(GetUserByIDHandler).Assembly);

            IConfigurationSection connectionStringSettings = config.GetSection("ConnectionStrings");
            builder.Services.Configure<ConnectionStrings>(connectionStringSettings);

            IConfigurationSection applicationConfigSettings = config.GetSection("ApplicationConfig");
            builder.Services.Configure<ApplicationConfig>(applicationConfigSettings);

            var sqlConnectionString = config.GetConnectionString("SqlConnectionString");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(sqlConnectionString));

            builder.Services.AddTransient<IRepository, Repository>();
            builder.Services.AddTransient<IAddressService, AddressService>();
            builder.Services.AddTransient<ICommunicationService, CommunicationService>();
            builder.Services.AddTransient<IHelperService, HelperService>();
            builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
            builder.Services.AddTransient<IDistanceCalculator, DistanceCalculator>();
            builder.Services.AddTransient<IVolunteerCache, VolunteerCache>();
            builder.Services.AddTransient<IVolunteersForCacheGetter, VolunteersForCacheGetter>();
            builder.Services.AddSingleton<IPollyMemoryCacheProvider, PollyMemoryCacheProvider>();
            builder.Services.AddTransient<ISystemClock, MockableDateTime>();
            builder.Services.AddTransient<ICoordinatedResetCache, CoordinatedResetCache>();
            builder.Services.AddTransient<IVolunteersFilteredByMinDistanceGetter, VolunteersFilteredByMinDistanceGetter>();
            builder.Services.AddTransient<IMinDistanceFilter, MinDistanceFilter>();

            // add cache
            RedisConfig redisConfig = new RedisConfig();
            config.GetSection("RedisConfig").Bind(redisConfig);
            builder.Services.AddMemDistCache(redisConfig.AppName, redisConfig.ConnectionString);
            
            builder.Services.AddSingleton<IMemDistCache<IEnumerable<CachedVolunteerDto>>>(x => x.GetService<IMemDistCacheFactory<IEnumerable<CachedVolunteerDto>>>().GetCache(new TimeSpan(7, 0, 0, 0), ResetTimeFactory.OnHour));


            // automatically apply EF migrations
            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbContextOptionsBuilder.UseSqlServer(sqlConnectionString);
            ApplicationDbContext dbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options);

            dbContext.Database.Migrate();

        }
    }
}