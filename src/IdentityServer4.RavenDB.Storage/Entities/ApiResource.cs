using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Entities
{
    internal class ApiResource : Resource
    {
        public List<string> AllowedAccessTokenSigningAlgorithms { get; set; }
        public List<Secret> Secrets { get; set; }
        public List<string> Scopes { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool NonEditable { get; set; }

        protected override string FormatDocumentId(string name) => "ApiResources/" + name;
    }
}
