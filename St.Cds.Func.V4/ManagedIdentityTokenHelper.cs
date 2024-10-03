using Azure.Core;
using Azure.Identity;
using System;
using System.Threading.Tasks;

namespace St.Cds.Func.V4
{
    internal static class ManagedIdentityTokenHelper
    {
        public static async Task<string> GetTokenAsync(string instanceUri)
        {
            var managedIdentityCredential = new ManagedIdentityCredential(Configuration.ClientId);

            var properScope = new Uri(instanceUri).GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
            var acessToken = await managedIdentityCredential.GetTokenAsync(new TokenRequestContext(new[] { properScope }));

            return acessToken.Token;
        }
    }
}
