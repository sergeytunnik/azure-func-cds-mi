using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerPlatform.Dataverse.Client;
using St.Cds.Func.V4;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IOrganizationServiceAsync2>(_ =>
            new ServiceClient(new Uri(Configuration.DataverseInstance), ManagedIdentityTokenHelper.GetTokenAsync, true));
    })
    .Build();

host.Run();
