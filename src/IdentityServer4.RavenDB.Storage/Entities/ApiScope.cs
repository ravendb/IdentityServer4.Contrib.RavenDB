namespace IdentityServer4.RavenDB.Storage.Entities
{
    public class ApiScope : Resource
    {
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
    }
}
