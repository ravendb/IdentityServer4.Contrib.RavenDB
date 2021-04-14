using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal interface IDocumentStoreHolder
    {
        public IAsyncDocumentSession OpenAsyncSession();
    }
}