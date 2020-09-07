using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.Extensions
{
    public static class SqlConnectionExtensions
    {
        public static void AddAzureToken(this SqlConnection connection)
        {
            if (connection.DataSource.Contains("database.windows.net"))
            {
                connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
            }
        }
    }
}
