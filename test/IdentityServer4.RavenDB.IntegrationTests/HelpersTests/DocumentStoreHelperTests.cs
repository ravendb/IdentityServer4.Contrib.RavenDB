using System;
using IdentityServer4.RavenDB.Storage.Helpers;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.HelpersTests
{
    public class DocumentStoreHelperTests
    {
        [Fact]
        public void InitializeDocumentStore_Throws_WhenDatabaseNameIsNotProvided()
        {
            Assert.Throws<InvalidOperationException>(() => DocumentStoreHelper.InitializeDocumentStore(store =>
            {
                store.Database = null;
                store.Urls = new[]
                {
                    "http://some-url"
                };
            }));
        }

        [Fact]
        public void InitializeDocumentStore_Throws_WhenUrlsIsEmptyCollection()
        {
            Assert.Throws<InvalidOperationException>(() => DocumentStoreHelper.InitializeDocumentStore(store =>
            {
                store.Database = "some-database-name";
                store.Urls = new string [] { };
            }));
        }
    }
}