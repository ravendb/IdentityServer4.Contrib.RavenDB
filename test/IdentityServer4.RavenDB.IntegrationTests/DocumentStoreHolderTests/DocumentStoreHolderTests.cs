using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using Raven.Client.Documents;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.DocumentStoreHolderTests
{
    public class DocumentStoreHolderTests
    {
        [Fact]
        public void DocumentStore_IsDisposed_WhenConfigurationDocumentStoreHolderIsDisposed()
        {
            var store = ConfigureDocumentStore();
            
            using (GetConfigurationDocumentStoreHolder(store))
            {
                AssertDocumentStoreNotDisposed(store);
            }

            AssertDocumentStoreDisposed(store);
        }

        [Fact]
        public void DocumentStore_IsDisposed_WhenOperationalDocumentStoreHolderIsDisposed()
        {
            var store = ConfigureDocumentStore();
            
            using (GetOperationalDocumentStoreHolder(store))
            {
                AssertDocumentStoreNotDisposed(store);
            }

            AssertDocumentStoreDisposed(store);
        }
        
        [Fact]
        public void Certificate_IsDisposed_WhenConfigurationDocumentStoreHolderIsDisposed()
        {
            var certificate = GetCertificate();
            var store = ConfigureDocumentStore(certificate);
            
            AssertCertificateCreated(store);

            using (GetConfigurationDocumentStoreHolder(store))
            {
            }
            
            AssertCertificateDisposed(store);
        }

        [Fact]
        public void Certificate_IsDisposed_WhenOperationalDocumentStoreHolderIsDisposed()
        {
            var certificate = GetCertificate();
            var store = ConfigureDocumentStore(certificate);
            
            AssertCertificateCreated(store);

            using (GetOperationalDocumentStoreHolder(store))
            {
            } 
            
            AssertCertificateDisposed(store);
        }

        private X509Certificate2 GetCertificate()
        {
            const string certificatePath =
                "IdentityServer4.RavenDB.IntegrationTests.DocumentStoreHolderTests.certificate.txt";

            var cert = EmbeddedResourceHelper.GetFileContent(certificatePath);

            var bytes = Convert.FromBase64String(cert);
            
            return new X509Certificate2(bytes);
        }

        private DocumentStore ConfigureDocumentStore(X509Certificate2 certificate = null)
        {
            return DocumentStoreHelper.InitializeDocumentStore(store =>
            {
                store.Database = "some-database-name";
                store.Urls = new[]
                {
                    "https://127.0.0.1"
                };
                store.Certificate = certificate;
            });
        }

        private ConfigurationDocumentStoreHolder GetConfigurationDocumentStoreHolder(IDocumentStore store)
        {
            return new ConfigurationDocumentStoreHolder(store);
        }
        
        private OperationalDocumentStoreHolder GetOperationalDocumentStoreHolder(IDocumentStore store)
        {
            return new OperationalDocumentStoreHolder(store);
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

        private void AssertCertificateDisposed(DocumentStore store)
        {
            Assert.Throws<CryptographicException>(() => store.Certificate.RawData);
            Assert.Throws<CryptographicException>(() => store.Certificate.PublicKey);
            Assert.Throws<CryptographicException>(() => store.Certificate.Thumbprint);
        }
    }
}