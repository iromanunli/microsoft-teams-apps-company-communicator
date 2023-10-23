using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

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
            var UserId = this.User.Identity.Name;
            var entryDate = DateTime.Now;
            var storAccount = Environment.GetEnvironmentVariable("StorageAccount");



            TableClient tableClient = new TableClient(storAccount, "Usage");

            TableEntity tableEntity = new TableEntity(UserId, notificationId)
            {
                {"entryDate", entryDate.ToString() }
            };

            tableClient.AddEntity(tableEntity);

            HttpContext.Response.Redirect(redirect);
        }
    }
}