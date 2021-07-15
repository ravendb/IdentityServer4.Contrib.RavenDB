using System.Collections.Generic;
using IdentityServer4.RavenDB.Storage.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace IdentityServer4.RavenDB.Storage.Helpers
{
    internal static class IndexHelper
    {
        private static readonly IReadOnlyList<AbstractIndexCreationTask> ConfigurationStoreIndexes = new List<AbstractIndexCreationTask>
        {
            new ClientIndex(),
            new ApiResourceIndex(),
            new ApiScopeIndex(),
            new IdentityResourceIndex()
        };
        
        private static readonly IReadOnlyList<AbstractIndexCreationTask> OperationalStoreIndexes = new List<AbstractIndexCreationTask>
        {
            new PersistedGrantIndex(), 
            new DeviceFlowCodeIndex(),
        };

        public static void ExecuteConfigurationStoreIndexes(IDocumentStore store)
        {
            ExecuteIndexes(store, ConfigurationStoreIndexes);
        }

        public static void ExecuteOperationalStoreIndexes(IDocumentStore store)
        {
            ExecuteIndexes(store, OperationalStoreIndexes);
        }
        
        private static void ExecuteIndexes(IDocumentStore store, IEnumerable<AbstractIndexCreationTask> indexes)
        {
            foreach (var index in indexes)
            { 
                store.ExecuteIndex(index, store.Database);
            }
        }
    }
}