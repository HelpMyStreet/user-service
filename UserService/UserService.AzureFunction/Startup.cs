using AutoMapper;
using UserService.Core.Interfaces.Repositories;
using UserService.Handlers;
using UserService.Mappers;
using UserService.Repo;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using UserService.Core;
using UserService.Core.Config;
using UserService.Core.Interfaces.Services;
using UserService.Core.Interfaces.Utils;
using UserService.Core.Services;
using UserService.Core.Utils;
using Microsoft.AspNetCore.Mvc.Formatters.Json;


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


            Dictionary<HttpClientConfigName, ApiConfig> httpClientConfigs = config.GetSection("Apis").Get<Dictionary<HttpClientConfigName, ApiConfig>>();

            foreach (KeyValuePair<HttpClientConfigName, ApiConfig> httpClientConfig in httpClientConfigs)
            {
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
                });

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
            builder.Services.AddTransient<IAddressService, Core.Services.AddressService>();
            builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
            builder.Services.AddTransient<IDistanceCalculator, DistanceCalculator>();
            builder.Services.AddTransient<IVolunteerCache, VolunteerCache>();
            builder.Services.AddTransient<IVolunteersForCacheGetter, VolunteersForCacheGetter>();
            builder.Services.AddSingleton<IPollyMemoryCacheProvider, PollyMemoryCacheProvider>();
            builder.Services.AddTransient<ISystemClock, MockableDateTime>();
        }
    }
}