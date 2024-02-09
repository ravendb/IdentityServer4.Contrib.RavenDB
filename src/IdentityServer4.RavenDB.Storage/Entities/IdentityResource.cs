using System;

namespace IdentityServer4.RavenDB.Storage.Entities
{
    public sealed class IdentityResource : Resource
    {
        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime? Updated { get; set; }

        public bool NonEditable { get; set; }

        protected override string FormatDocumentId(string name) => "IdentityResources/" + name;
    }
}
