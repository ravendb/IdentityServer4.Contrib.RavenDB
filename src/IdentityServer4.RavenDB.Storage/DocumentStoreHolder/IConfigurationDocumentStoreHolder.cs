using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal interface IConfigurationDocumentStoreHolder : IDocumentStoreHolder
    {
    }
}