using System.Security.Claims;

namespace IdentityServer4.RavenDB.Storage.Entities
{
    public sealed class ClientClaim
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; } = ClaimValueTypes.String;
    }
}
