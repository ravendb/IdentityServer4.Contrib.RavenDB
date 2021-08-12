using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.RavenDB.Storage.Helpers;
using Raven.Client.Documents;
using Xunit;

namespace IdentityServer4.RavenDB.Storage.Tests.DocumentStoreHolderTests
{
    public class DocumentStoreHolderTests : IntegrationTestBase
    {
        [Fact]
        public void DocumentStore_IsDisposed_WhenConfigurationDocumentStoreHolderIsDisposed()
        {
            IDocumentStore documentStore;

            using(var storeHolder =  GetConfigurationDocumentStoreHolder())
            {
                documentStore = storeHolder.IntegrationTest_GetDocumentStore();
                AssertDocumentStoreNotDisposed(documentStore);
            }

            AssertDocumentStoreDisposed(documentStore);
        }

        [Fact]
        public void DocumentStore_IsDisposed_WhenOperationalDocumentStoreHolderIsDisposed()
        {
            IDocumentStore documentStore;
            
            using (var storeHolder = GetOperationalDocumentStoreHolder())
            {
                documentStore = storeHolder.IntegrationTest_GetDocumentStore();
                AssertDocumentStoreNotDisposed(documentStore);
            }

            AssertDocumentStoreDisposed(documentStore);
        }
        
        private void AssertDocumentStoreDisposed(IDocumentStore store)
        {
            Assert.True(store.WasDisposed);
        }

        private void AssertDocumentStoreNotDisposed(IDocumentStore store)
        {
            Assert.False(store.WasDisposed);
        }
    }
}