using System;
using System.Collections.Generic;

namespace IdentityServer4.RavenDB.Entities
{
    public class IdentityResource
    {
        public string Id { get; set; }
        
        public bool Enabled { get; set; } = true;
        
        public string Name { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public bool Required { get; set; }
        
        public bool Emphasize { get; set; }
        
        public bool ShowInDiscoveryDocument { get; set; } = true;
        
        public List<string> UserClaims { get; set; }
        
        public List<Property> Properties { get; set; }
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        
        public DateTime? Updated { get; set; }
        
        public bool NonEditable { get; set; }
    }
}
