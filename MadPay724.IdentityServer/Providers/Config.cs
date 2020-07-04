using IdentityServer4.Models;
using System.Collections.Generic;

namespace MadPay724.IdentityServer.Providers
{
    // http://localhost:5000/.well-known/openid-configuration
    public class Config
    {
        public static IEnumerable<ApiResource> GetAllApiResorces()
        {
            return new List<ApiResource>
            {
                new ApiResource("MadPay724Api", "Customer Api For MadPay724")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client{
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "MadPay724Api" }
                }
            };
        }
    }
}
