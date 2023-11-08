using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Data.Tables;

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func
{
    internal class QueryUsage
    {
        public static string QueryData(string notificationId)
        {
            StringBuilder usages = new StringBuilder();

            var serviceClient = new TableServiceClient(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
            TableClient table = serviceClient.GetTableClient("Usage");

            var queryResult = table.Query<Usage>(filter: $"notificationId eq '{notificationId}'").ToArray();

            foreach (Usage u in queryResult)
            {
                usages.AppendFormat("{0}-{1}-{2};", u.notificationId, u.userId, u.entryDate);
            }

            return usages.ToString();
        }
    }
}
