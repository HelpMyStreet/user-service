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

[assembly: FunctionsStartup(typeof(UserService.AzureFunction.Startup))]
namespace UserService.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddMediatR(typeof(GetUserByIDHandler).Assembly);
            //builder.Services.AddAutoMapper(typeof(AddressDetailsProfile).Assembly);

            var tmpConfig = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables()
            .Build();

            var sqlConnectionString = tmpConfig.GetConnectionString("SqlConnectionString");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(sqlConnectionString));

            builder.Services.AddTransient<IRepository, Repository>();
        }
    }
}