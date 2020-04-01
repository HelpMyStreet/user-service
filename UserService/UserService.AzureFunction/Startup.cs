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

[assembly: FunctionsStartup(typeof(UserService.AzureFunction.Startup))]
namespace UserService.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddMediatR(typeof(FunctionAHandler).Assembly);
            //builder.Services.AddAutoMapper(typeof(AddressDetailsProfile).Assembly);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseInMemoryDatabase(databaseName: "UserService.AzureFunction"));
            builder.Services.AddTransient<IRepository, Repository>();
        }
    }
}