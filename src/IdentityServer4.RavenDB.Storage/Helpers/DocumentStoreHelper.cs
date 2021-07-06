using System;
using Raven.Client.Documents;

namespace IdentityServer4.RavenDB.Storage.Helpers
{
    internal static class DocumentStoreHelper
    {
        public static DocumentStore InitializeDocumentStore(Action<DocumentStore> configureDocumentStore)
        {
            var documentStore = new DocumentStore();
            configureDocumentStore(documentStore);

            var databaseName = documentStore.Database;
            var urls = documentStore.Urls;

            if (databaseName == null)
            {
                throw new InvalidOperationException($"{nameof(documentStore.Database)} must be provided when setting up Identity Server configuration and operational RavenDb stores." );
            }

            if (urls.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(documentStore.Urls)} cannot be empty when setting up Identity Server configuration and operational RavenDb stores.");
            }

            if (documentStore.Certificate != null)
            {
                documentStore.AfterDispose += (sender, args) =>
                {
                    documentStore.Certificate.Dispose();
                };
            }
            
            documentStore.Initialize();

            return documentStore;
        }
    }
}