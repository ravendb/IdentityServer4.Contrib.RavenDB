using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal abstract class DocumentStoreHolderBase : IDocumentStoreHolder
    {
        public readonly IDocumentStore DocumentStore;

        protected DocumentStoreHolderBase(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public IAsyncDocumentSession OpenAsyncSession() => DocumentStore.OpenAsyncSession();
    }
}