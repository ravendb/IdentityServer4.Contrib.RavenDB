using IdentityServer4.Models;

namespace IdentityServer4.RavenDB.Storage.Helpers
{
    public static class CryptographyHelper
    {
        public static string CreateHash(string value)
        {
            return value.Sha256();
        }
    }
}