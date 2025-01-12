﻿// <copyright file="DraftNotificationSummaryForConsent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Draft notification summary (for consent page) model class.
    /// </summary>
    public class DraftNotificationSummaryForConsent
    {
        /// <summary>
        /// Gets or sets Notification Id value.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets Team Names value.
        /// </summary>
        public IEnumerable<string> TeamNames { get; set; }

        /// <summary>
        /// Gets or sets Roster Names value.
        /// </summary>
        public IEnumerable<string> RosterNames { get; set; }

        /// <summary>
        /// Gets or sets Group Names value.
        /// </summary>
        public IEnumerable<string> GroupNames { get; set; }
    }
}
