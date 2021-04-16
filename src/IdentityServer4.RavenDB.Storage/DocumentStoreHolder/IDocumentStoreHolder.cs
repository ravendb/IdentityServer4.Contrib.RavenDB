using System.Runtime.CompilerServices;
using Raven.Client.Documents.Session;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal interface IDocumentStoreHolder
    {
        public IAsyncDocumentSession OpenAsyncSession();
    }
}