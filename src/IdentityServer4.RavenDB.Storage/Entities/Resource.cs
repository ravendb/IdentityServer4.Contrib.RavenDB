using System.Collections.Generic;
using System.Diagnostics;

namespace IdentityServer4.RavenDB.Storage.Entities
{
    /// <summary>
    /// Models the common data of API and identity resources.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Resource
    {
        private string DebuggerDisplay => Name ?? $"{{{typeof(Resource)}}}";

        public string Id { get; set; }
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<string> UserClaims { get; set; }
        public List<Property> Properties { get; set; }
    }
}