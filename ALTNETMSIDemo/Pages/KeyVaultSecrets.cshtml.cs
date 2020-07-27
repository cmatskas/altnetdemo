using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace ALTNETMSIDemo.Pages
{
    public class KeyVaultSecretsModel : PageModel
    {

        private readonly IConfiguration configuration;
        public string FromKeyVault { get; set; }
        public string FromConfig { get; set; }

        public KeyVaultSecretsModel(IConfiguration config)
        {
            configuration = config;
        }
        public void OnGet()
        {
            var clientId = configuration["ClientId"];

            var managedIdentityCredential = string.IsNullOrEmpty(clientId)
                ? new ManagedIdentityCredential()
                : new ManagedIdentityCredential(clientId);

            var credential = new ChainedTokenCredential(
                new VisualStudioCredential(), 
                new AzureCliCredential(), 
                managedIdentityCredential);

            var client = new SecretClient(vaultUri: new Uri(configuration["KeyVaultUrl"]), credential: credential);
            // Retrieve a secret using the secret client.
            FromKeyVault = client.GetSecret("KVSercret").Value.Value;

            // Retrive secret from config stored in Key Vault
            FromConfig = configuration["SomeConfigValueFromKV"];
            
        }
    }
}
