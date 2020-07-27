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

        private readonly ILogger<KeyVaultSecretsModel> _logger;
        private readonly IConfiguration configuration;
        public string FromKeyVault { get; set; }
        public string FromConfig { get; set; }

        public KeyVaultSecretsModel(ILogger<KeyVaultSecretsModel> logger, IConfiguration config)
        {
            configuration = config;
            _logger = logger;
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

            //KVSercret
            var client = new SecretClient(vaultUri: new Uri(configuration["KeyVaultUrl"]), credential: credential);
            // Retrieve a secret using the secret client.
            FromKeyVault = client.GetSecret("KVSercret").Value.Value;
            
        }
    }
}
