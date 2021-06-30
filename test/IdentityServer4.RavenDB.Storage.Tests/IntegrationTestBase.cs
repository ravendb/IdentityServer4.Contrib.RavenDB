using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.TestDriver;

namespace IdentityServer4.RavenDB.Storage.Tests
{
    public class IntegrationTestBase : RavenTestDriver
    {
        internal ConfigurationDocumentStoreHolder GetConfigurationDocumentStoreHolder()
        {
            var options = GetRavenDbConfigurationStoreOptions();
            return new ConfigurationDocumentStoreHolder(options);
        }

        internal RavenDbConfigurationStoreOptions GetRavenDbConfigurationStoreOptions(X509Certificate2 cert = null)
        {
            var documentStore = GetDocumentStore();
            
            var options = new RavenDbConfigurationStoreOptions
            {
                ConfigureDocumentStore = store =>
                {
                    store.Database = documentStore.Database;
                    store.Urls = documentStore.Urls;
                    store.Certificate = cert;
                }
            };

            return options;
        }


        internal OperationalDocumentStoreHolder GetOperationalDocumentStoreHolder()
        {
            var options = GetRavenDbOperationalStoreOptions();
            return new OperationalDocumentStoreHolder(options);
        }

        internal RavenDbOperationalStoreOptions GetRavenDbOperationalStoreOptions()
        {
            var documentStore = GetDocumentStore();
            
            var options = new RavenDbOperationalStoreOptions
            {
                ConfigureDocumentStore = store =>
                {
                    store.Database = documentStore.Database;
                    store.Urls = documentStore.Urls;
                }
            };

            return options;
        }

        internal Task ExecuteIndex(IDocumentStore store, AbstractIndexCreationTask index) =>
            index.ExecuteAsync(store);
    }
}
