using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ALTNETMSIDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    var clientId = builtConfig["ClientId"];

                    var managedIdentityCredential = string.IsNullOrEmpty(clientId)
                        ? new ManagedIdentityCredential()
                        : new ManagedIdentityCredential(clientId);

                    var credential = new ChainedTokenCredential(
                                        new VisualStudioCredential(),
                                        new AzureCliCredential(),
                                        managedIdentityCredential);

                    config.AddAzureKeyVault(new Uri(builtConfig["KeyVaultUrl"]), credential);
                })
               .UseStartup<Startup>();
    }
}
