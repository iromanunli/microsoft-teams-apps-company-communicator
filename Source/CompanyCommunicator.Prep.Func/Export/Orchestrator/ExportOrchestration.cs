﻿// <copyright file="ExportOrchestration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Orchestrator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.PreparingToSend;

    /// <summary>
    /// This class is the durable framework orchestration for exporting notifications.
    /// </summary>
    public static class ExportOrchestration
    {
        /// <summary>
        /// This is the durable orchestration method,
        /// which starts the export process.
        /// </summary>
        /// <param name="context">Durable orchestration context.</param>
        /// <param name="log">Logging service.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.ExportOrchestration)]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var exportRequiredData = context.GetInput<ExportDataRequirement>();
            var sentNotificationDataEntity = exportRequiredData.NotificationDataEntity;
            var exportDataEntity = exportRequiredData.ExportDataEntity;

            if (!context.IsReplaying)
            {
                log.LogInformation($"Start to export the notification {sentNotificationDataEntity.Id}!");
            }

            try
            {
                if (!context.IsReplaying)
                {
                    log.LogInformation("About to update export is in progress.");
                }

                exportDataEntity.Status = ExportStatus.InProgress.ToString();
                await context.CallActivityWithRetryAsync(
                    FunctionNames.UpdateExportDataActivity,
                    FunctionSettings.DefaultRetryOptions,
                    exportDataEntity);

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to get the metadata information.");
                }

                var metaData = await context.CallActivityWithRetryAsync<Metadata>(
                    FunctionNames.GetMetadataActivity,
                    FunctionSettings.DefaultRetryOptions,
                    (sentNotificationDataEntity, exportDataEntity));

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to start file upload.");
                }

                StringBuilder usages = new StringBuilder();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable usageTable = tableClient.GetTableReference(Environment.GetEnvironmentVariable("StorageAccountName"));

#pragma warning disable SA1119 // Statement should not use unnecessary parenthesis
                var query = (from usage in usageTable.CreateQuery<Usage>()
                             where sentNotificationDataEntity.Id == usage.NotificationId
                             select usage);
#pragma warning restore SA1119 // Statement should not use unnecessary parenthesis

                foreach (Usage u in query)
                {
                    usages.AppendFormat("{0}-{1}-{2};", u.NotificationId, u.UserId, u.entryDate);
                }

                await context.CallActivityWithRetryAsync(
                    FunctionNames.UploadActivity,
                    FunctionSettings.DefaultRetryOptions,
                    (sentNotificationDataEntity, metaData, usages.ToString(), exportDataEntity.FileName));

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to send file card.");
                }

                var consentId = await context.CallActivityWithRetryAsync<string>(
                    FunctionNames.SendFileCardActivity,
                    FunctionSettings.DefaultRetryOptions,
                    (exportRequiredData.UserId, exportRequiredData.NotificationDataEntity.Id, exportDataEntity.FileName));

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to update export is completed.");
                }

                exportDataEntity.FileConsentId = consentId;
                exportDataEntity.Status = ExportStatus.Completed.ToString();
                await context.CallActivityWithRetryAsync(
                    FunctionNames.UpdateExportDataActivity,
                    FunctionSettings.DefaultRetryOptions,
                    exportDataEntity);

                log.LogInformation($"ExportOrchestration is successful for notification id:{sentNotificationDataEntity.Id}!");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Failed to export notification {sentNotificationDataEntity.Id} : {ex.Message}");
                await context.CallActivityWithRetryAsync(
                    FunctionNames.HandleExportFailureActivity,
                    FunctionSettings.DefaultRetryOptions,
                    exportDataEntity);
            }
        }
    }
}