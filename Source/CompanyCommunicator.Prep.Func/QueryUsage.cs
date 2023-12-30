using Azure.Data.Tables;
using Dynamitey.DynamicObjects;
using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func
{
    internal class QueryUsage
    {
        public static List<Usos> QueryData(string notificationId)
        {
            StringBuilder usages = new StringBuilder();

            var serviceClient = new TableServiceClient(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
            TableClient table = serviceClient.GetTableClient("Usage");
            table.CreateIfNotExists();

            var queryResult = table.Query<Usage>(filter: $"notificationId eq '{notificationId}'").ToArray();

            List<Usos> lstUsos = new List<Usos>();

            foreach (Usage u in queryResult)
            {
                lstUsos.Add(new Usos()
                {
                    entryDate = u.entryDate,
                    notificationId = u.notificationId,
                    userId = u.userId,
                });
            }

            return lstUsos;
        }
    }
}
