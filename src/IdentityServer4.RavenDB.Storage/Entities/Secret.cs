using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Entities
{
    internal class Secret
    {
        public string Description { get; set; }

        public string Value { get; set; }

        public DateTime? Expiration { get; set; }

        public string Type { get; set; } = "SharedSecret";

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
