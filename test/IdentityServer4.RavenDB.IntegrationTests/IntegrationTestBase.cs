using System.Threading.Tasks;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.TestDriver;

namespace IdentityServer4.RavenDB.IntegrationTests
{
    public class IntegrationTestBase : RavenTestDriver
    {
        internal ConfigurationDocumentStoreHolder GetConfigurationDocumentStoreHolder() => 
            new ConfigurationDocumentStoreHolder(GetDocumentStore());
        
        internal OperationalDocumentStoreHolder GetOperationalDocumentStoreHolder() => 
            new OperationalDocumentStoreHolder(GetDocumentStore());

        internal Task ExecuteIndex(IDocumentStore store, AbstractIndexCreationTask index) =>
            index.ExecuteAsync(store);
    }
}
