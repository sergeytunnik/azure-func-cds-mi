using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Cds.Client;
using Azure.Identity;
using Azure.Core;
using Microsoft.Crm.Sdk.Messages;
using System.Threading;

namespace ST.Cds.Func.V3
{
    public static class Functions
    {
        private const string clientId = "<managed identity client id>";
        private const string cdsUri = "https://org.crm4.dynamics.com/";

        private static int counter = 0;

        [FunctionName("WhoAmI")]
        public static async Task<IActionResult> WhoAmI(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            Interlocked.Increment(ref counter);
            log.LogInformation($"Counter: {counter}.");
            try
            {
                log.LogInformation("Creating CdsServiceClient.");

                var cdsServiceClient = new CdsServiceClient(new Uri(cdsUri), GetTokenAsync, true);

                log.LogInformation("Executing WhoAmI request.");
                var whoAmIResponse = (WhoAmIResponse)cdsServiceClient.Execute(new WhoAmIRequest());

                log.LogInformation("Returning result.");
                return new OkObjectResult(whoAmIResponse);
            } catch (Exception ex)
            {
                log.LogError(ex, "Something wrong.");
                return new BadRequestObjectResult(ex);
            }
        }

        private static async Task<string> GetTokenAsync(string instanceUri)
        {
            var managedIdentityCredential = new ManagedIdentityCredential(clientId);

            var properScope = new Uri(instanceUri).GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
            var acessToken = await managedIdentityCredential.GetTokenAsync(new TokenRequestContext(new[] { properScope }));

            return acessToken.Token;
        }
    }
}
