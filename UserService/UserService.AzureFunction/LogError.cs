using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.AzureFunction
{
    public static class LogError
    {
        public static void Log(ILogger log, Exception exc, Object request)
        {
            string jsonRequest = JsonConvert.SerializeObject(request);
            log.LogInformation(jsonRequest);
            log.LogInformation(exc.ToString());
        }
    }
}
