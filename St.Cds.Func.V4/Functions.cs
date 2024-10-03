using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace St.Cds.Func.V4
{
    public class Functions
    {
        private static int counter = 0;

        private readonly ILogger<Functions> logger;
        private readonly IOrganizationServiceAsync2 organizationService;

        public Functions(ILogger<Functions> logger, IOrganizationServiceAsync2 organizationService)
        {
            this.logger = logger;
            this.organizationService = organizationService;
        }

        [Function("WhoAmI")]
        public async Task<IActionResult> WhoAmI(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            Interlocked.Increment(ref counter);
            logger.LogInformation($"Counter: {counter}.");
            try
            {
                logger.LogInformation("Executing WhoAmI request.");
                var whoAmIResponse = (WhoAmIResponse)await organizationService.ExecuteAsync(new WhoAmIRequest());

                logger.LogInformation("Returning result.");
                return new OkObjectResult(whoAmIResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something wrong.");
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
