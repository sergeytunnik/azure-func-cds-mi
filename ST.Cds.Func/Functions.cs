using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;

namespace ST.Cds.Func
{
    public static class Functions
    {
        private const string clientId = "<managed identity client id>";
        private const string cdsUri = "https://org.crm4.dynamics.com/";

        static Functions()
        {
            AssemblyBindingRedirectHelper.ConfigureBindingRedirects();

            // Configuring external authentication for CrmServiceClient.
            CrmServiceClient.AuthOverrideHook = new AzureFuncOverrideAuthHookWrapper(clientId);
        }

        [FunctionName("WhoAmI")]
        public static async Task<HttpResponseMessage> WhoAmI([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("Creating CrmServiceClient.");
                var crmServiceClient = new CrmServiceClient(new Uri(cdsUri), true);

                log.Info("Executing WhoAmI request.");
                var whoAmIResponse = (WhoAmIResponse)crmServiceClient.Execute(new WhoAmIRequest());

                log.Info("Returning result.");
                return req.CreateResponse(HttpStatusCode.OK, whoAmIResponse);
            }
            catch (Exception ex)
            {
                log.Error($"Ex {ex.Message}. Inner: {ex.InnerException?.Message}.", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, $"Ex {ex.Message}. Inner: {ex.InnerException?.Message}.");
            }
        }
    }
}
