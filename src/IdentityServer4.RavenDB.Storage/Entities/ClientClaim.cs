using System.Runtime.CompilerServices;
using System.Security.Claims;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Entities
{
    internal class ClientClaim
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; } = ClaimValueTypes.String;
    }
}
