using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Entities
{
    internal class Property
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
