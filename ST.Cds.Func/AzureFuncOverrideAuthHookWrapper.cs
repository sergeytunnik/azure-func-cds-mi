using Azure.Core;
using Azure.Identity;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace ST.Cds.Func
{
    class AzureFuncOverrideAuthHookWrapper : IOverrideAuthHookWrapper
    {
        private readonly ManagedIdentityCredential managedIdentityCredential;

        public AzureFuncOverrideAuthHookWrapper(string clientId)
        {
            this.managedIdentityCredential = new ManagedIdentityCredential(clientId);
        }

        public string GetAuthToken(Uri connectedUri)
        {
            var properScope = connectedUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);

            var acessToken = managedIdentityCredential.GetToken(new TokenRequestContext(new[] { properScope }));

            return acessToken.Token;
        }
    }
}
