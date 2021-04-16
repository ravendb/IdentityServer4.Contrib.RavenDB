using System.Runtime.CompilerServices;
using Raven.Client.Documents;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal class OperationalDocumentStoreHolder : DocumentStoreHolderBase, IOperationalDocumentStoreHolder
    {
        public OperationalDocumentStoreHolder(IDocumentStore documentStore) : base(documentStore)
        {
        }
    }
}