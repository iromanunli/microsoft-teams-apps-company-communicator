using Azure.Data.Tables;
using System;
using System.Linq;
using System.Text;

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

            usages.Append("***");

            foreach (Usage u in queryResult)
            {
                usages.AppendFormat("{0}-{1}-{2};", u.notificationId, u.userId, u.entryDate);
            }

            usages.Append("***");

            return usages.ToString();
        }
    }
}
