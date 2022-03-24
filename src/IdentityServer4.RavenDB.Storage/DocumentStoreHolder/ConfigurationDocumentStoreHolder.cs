﻿using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal class ConfigurationDocumentStoreHolder : IDocumentStoreHolder
    {
        private readonly IDocumentStore _documentStore;

        public ConfigurationDocumentStoreHolder(RavenDbConfigurationStoreOptions options)
            : this(DocumentStoreHelper.InitializeDocumentStore(options.ConfigureDocumentStore), options)
        {
        }

        public ConfigurationDocumentStoreHolder(IDocumentStore documentStore, RavenDbConfigurationStoreOptions options)
        {
            _documentStore = documentStore;
            if (options.CreateIndexes)
            {
                IndexHelper.ExecuteConfigurationStoreIndexes(_documentStore);
            }
        }

        public IDocumentStore IntegrationTest_GetDocumentStore() => _documentStore;

        public IAsyncDocumentSession OpenAsyncSession() => _documentStore.OpenAsyncSession();

        public void Dispose()
        {
            _documentStore?.Dispose();
        }
    }
}