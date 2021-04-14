using Raven.Client.Documents;

namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal class ConfigurationDocumentStoreHolder : DocumentStoreHolderBase, IConfigurationDocumentStoreHolder
    {
        public ConfigurationDocumentStoreHolder(IDocumentStore documentStore) : base(documentStore)
        {
        }
    }
}