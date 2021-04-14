using System.Security.Cryptography.X509Certificates;

namespace IdentityServer4.RavenDB.Storage.Options
{
    public class DocumentStoreOptions
    {
        public string Database { get; set; }
        public string[] Urls { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }
}