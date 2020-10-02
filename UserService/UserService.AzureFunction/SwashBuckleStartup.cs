using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using UserService.AzureFunction;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]
namespace UserService.AzureFunction
{
    internal class SwashBuckleStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
        }
    }
}
