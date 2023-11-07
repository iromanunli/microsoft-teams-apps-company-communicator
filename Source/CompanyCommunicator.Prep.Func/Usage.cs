using Azure;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func
{
    internal class Usage : ITableEntity
    {
        public string UserId { get; set; }
        public string NotificationId { get; set; }
        public DateTime entryDate { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        DateTimeOffset ITableEntity.Timestamp { get; set; }
        string ITableEntity.ETag { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            throw new NotImplementedException();
        }
    }
}
