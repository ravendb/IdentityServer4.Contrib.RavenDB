using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using Raven.Client.Documents;
using Xunit;

namespace IdentityServer4.RavenDB.Storage.Tests.DocumentStoreHolderTests
{
    public class DocumentStoreHolderTests : IntegrationTestBase
    {
        [Fact]
        public void DocumentStore_IsDisposed_WhenConfigurationDocumentStoreHolderIsDisposed()
        {
            var options = GetRavenDbConfigurationStoreOptions();

            IDocumentStore documentStore;

            using(var storeHolder =  GetConfigurationDocumentStoreHolder(options))
            {
                documentStore = storeHolder.IntegrationTest_GetDocumentStore();
                AssertDocumentStoreNotDisposed(documentStore);
            }

            AssertDocumentStoreDisposed(documentStore);
        }

        [Fact]
        public void DocumentStore_IsDisposed_WhenOperationalDocumentStoreHolderIsDisposed()
        {
            var options = GetRavenDbOperationalStoreOptions();
            
            IDocumentStore documentStore;
            
            using (var storeHolder = GetOperationalDocumentStoreHolder(options))
            {
                documentStore = storeHolder.IntegrationTest_GetDocumentStore();
                AssertDocumentStoreNotDisposed(documentStore);
            }

            AssertDocumentStoreDisposed(documentStore);
        }
        
        [Fact]
        public void Certificate_IsDisposed_WhenDocumentStoreIsDisposed()
        {
            var certificate = GetCertificate();

            IDocumentStore store;
            
            using (store = DocumentStoreHelper.InitializeDocumentStore(documentStore =>
            {
                documentStore.Urls = new[]
                {
                    "https://127.0.0.1:54987"
                };
                documentStore.Database = "some-database";
                documentStore.Certificate = certificate;
            }))
            {
                AssertDocumentStoreNotDisposed(store);
                AssertCertificateCreated(store);
            }
            
            AssertDocumentStoreDisposed(store);
            AssertCertificateDisposed(store);
        }

        private X509Certificate2 GetCertificate()
        {
            const string certificatePath =
                "IdentityServer4.RavenDB.Storage.Tests.DocumentStoreHolderTests.certificate.txt";

            var cert = EmbeddedResourceHelper.GetFileContent(certificatePath);

            var bytes = Convert.FromBase64String(cert);
            
            return new X509Certificate2(bytes);
        }

        private ConfigurationDocumentStoreHolder GetConfigurationDocumentStoreHolder(RavenDbConfigurationStoreOptions options)
        {
            return new ConfigurationDocumentStoreHolder(options);
        }
        
        private OperationalDocumentStoreHolder GetOperationalDocumentStoreHolder(RavenDbOperationalStoreOptions options)
        {
            return new OperationalDocumentStoreHolder(options);
        }

        private void AssertDocumentStoreDisposed(IDocumentStore store)
        {
            Assert.True(store.WasDisposed);
        }

        private void AssertDocumentStoreNotDisposed(IDocumentStore store)
        {
            Assert.False(store.WasDisposed);
        }

        private void AssertCertificateCreated(IDocumentStore store)
        {
            Assert.NotNull(store.Certificate.RawData);
            Assert.NotNull(store.Certificate.PublicKey);
            Assert.NotNull(store.Certificate.Thumbprint);
        }

        private void AssertCertificateDisposed(IDocumentStore store)
        {
            Assert.Throws<CryptographicException>(() => store.Certificate.RawData);
            Assert.Throws<CryptographicException>(() => store.Certificate.PublicKey);
            Assert.Throws<CryptographicException>(() => store.Certificate.Thumbprint);
        }
    }
}