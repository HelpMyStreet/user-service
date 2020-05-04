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
            NewRelic.Api.Agent.NewRelic.NoticeError(exc);
            log.LogInformation(exc.ToString());
        }
    }
}
