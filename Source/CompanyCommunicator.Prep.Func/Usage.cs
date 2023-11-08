using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func
{
    internal class Usage : ITableEntity
    {
        public string userId { get; set; }
        public string notificationId { get; set; }
        public DateTime entryDate { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
