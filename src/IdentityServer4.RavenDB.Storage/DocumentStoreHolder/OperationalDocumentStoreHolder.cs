using Raven.Client.Documents;

namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal class OperationalDocumentStoreHolder : DocumentStoreHolderBase, IOperationalDocumentStoreHolder
    {
        public OperationalDocumentStoreHolder(IDocumentStore documentStore) : base(documentStore)
        {
        }
    }
}