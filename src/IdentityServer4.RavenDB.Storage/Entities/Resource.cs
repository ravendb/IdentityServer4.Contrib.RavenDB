using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Entities
{
    /// <summary>
    /// Models the common data of API and identity resources.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class Resource
    {
        private string name;

        private string DebuggerDisplay => Name ?? $"{{{typeof(Resource)}}}";

        public string Id { get; private set; }

        public bool Enabled { get; set; } = true;

        public string Name
        {
            get => name;
            set
            {
                Id = FormatDocumentId(value);
                name = value;
            }
        }

        protected abstract string FormatDocumentId(string name);

        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<string> UserClaims { get; set; }
        public List<Property> Properties { get; set; }
    }
}