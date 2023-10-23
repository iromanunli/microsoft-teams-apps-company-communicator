using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.Teams.Apps.CompanyCommunicator.Redirector.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet([FromQuery(Name = "redirect")] string redirect, [FromQuery(Name = "notificationId")] string notificationId)
        {
            var userId = getEmail(HttpContext);

            var entryDate = DateTime.Now;
            var storAccount = Environment.GetEnvironmentVariable("StorageAccount");

            TableClient tableClient = new TableClient(storAccount, "Usage");

            TableEntity tableEntity = new TableEntity(Guid.NewGuid().ToString(), notificationId)
            {
                {"entryDate", entryDate.ToUniversalTime() },
                {"userId", userId  },
                {"notificationId", notificationId }
            };

            tableClient.AddEntity(tableEntity);

            HttpContext.Response.Redirect(redirect);
        }

        public static String getEmail(HttpContext context)
        {
            String identifier = "X-MS-CLIENT-PRINCIPAL-NAME";
            IEnumerable<string> headerValues = context.Request.Headers[identifier];
            if (headerValues == null)
            {
                return "";
            }
            else
            {
                return headerValues.FirstOrDefault();
            }
        }
    }
}