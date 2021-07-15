
namespace IdentityServer4.RavenDB.Storage.Entities
{
    internal class ApiScope : Resource
    {
        public bool Required { get; set; }
        public bool Emphasize { get; set; }

        protected override string FormatDocumentId(string name) => "ApiScopes/" + name;
    }
}
