using System.Runtime.CompilerServices;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal abstract class DocumentStoreHolderBase : IDocumentStoreHolder
    {
        public IDocumentStore DocumentStore;

        protected DocumentStoreHolderBase(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public IAsyncDocumentSession OpenAsyncSession() => DocumentStore.OpenAsyncSession();
    }
}