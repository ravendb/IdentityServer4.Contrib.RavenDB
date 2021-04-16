﻿using System.Collections.Generic;
using IdentityServer4.RavenDB.Storage.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace IdentityServer4.RavenDB.Storage.Helpers
{
    internal static class IndexHelper
    {
        public static readonly IReadOnlyList<AbstractIndexCreationTask> ConfigurationStoreIndexes = new List<AbstractIndexCreationTask>
        {
            new ClientIndex(),
            new ApiResourceIndex(),
            new ApiScopeIndex(),
            new IdentityResourceIndex()
        };
        
        public static readonly IReadOnlyList<AbstractIndexCreationTask> OperationalStoreIndexes = new List<AbstractIndexCreationTask>
        {
            new PersistentGrantIndex(), 
            new DeviceFlowCodeIndex(),
        };
        
        public static void ExecuteIndexes(IDocumentStore store, IEnumerable<AbstractIndexCreationTask> indexes)
        {
            foreach (var index in indexes)
            { 
                ExecuteIndex(store, index, store.Database);
            }
        }
        
        private static void ExecuteIndex(IDocumentStore store, AbstractIndexCreationTask index, string databaseName)
        {
            store.ExecuteIndex(index, databaseName);
        }
    }
}