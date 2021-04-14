using System;
using Raven.Client.Documents;

namespace IdentityServer4.RavenDB.Storage.Helpers
{
    internal static class DocumentStoreHelper
    {
        public static DocumentStore InitializeDocumentStore(Action<DocumentStore> configureDocumentStoreAction)
        {
            var documentStore = new DocumentStore();
            configureDocumentStoreAction(documentStore);

            var databaseName = documentStore.Database;
            var urls = documentStore.Urls;

            if (databaseName == null)
            {
                throw new InvalidOperationException($"Database name must be provided when setting up Identity Server configuration and operational RavenDb stores." );
            }

            if (urls.Length == 0)
            {
                throw new InvalidOperationException("Database url must be provided when setting up Identity Server configuration and operational RavenDb stores.");
            }

            documentStore.Initialize();

            return documentStore;
        }
    }
}