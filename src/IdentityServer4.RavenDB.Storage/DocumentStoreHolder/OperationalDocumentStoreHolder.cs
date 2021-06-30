using System;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal class OperationalDocumentStoreHolder : IDisposable
    {
        private readonly IDocumentStore _documentStore;
        
        public OperationalDocumentStoreHolder(RavenDbOperationalStoreOptions options)
        {
            _documentStore = DocumentStoreHelper.InitializeDocumentStore(options.ConfigureDocumentStore);
            IndexHelper.ExecuteOperationalStoreIndexes(_documentStore);
        }
        
        public IDocumentStore IntegrationTest_GetDocumentStore() => _documentStore;
        
        public IAsyncDocumentSession OpenAsyncSession() => _documentStore.OpenAsyncSession();

        public void Dispose()
        {
            _documentStore?.Dispose();
        }
    }
}