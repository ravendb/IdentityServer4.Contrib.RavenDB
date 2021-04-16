using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Indexes;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Services;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Services
{
    public class CorsPolicyServiceTests : IntegrationTestBase
    {
        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue()
        {
            const string testCorsOrigin = "https://identityserver.io/";

            var storeHolder = await GetOperationalDocumentStoreHolder_AndExecuteClientIndex();

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
                }.ToEntity());

                await session.StoreAsync(new Client
                {
                    ClientId = "2",
                    ClientName = "2",
                    AllowedCorsOrigins = new List<string> {"https://www.identityserver.com", testCorsOrigin}
                }.ToEntity());

                await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var service = new CorsPolicyService(storeHolder, FakeLogger<CorsPolicyService>.Create());
            var result = await service.IsOriginAllowedAsync(testCorsOrigin.ToUpperInvariant());
            
            Assert.True(result);
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse()
        {
            var storeHolder = await GetOperationalDocumentStoreHolder_AndExecuteClientIndex();

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
                }.ToEntity());

                await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var service = new CorsPolicyService(storeHolder, FakeLogger<CorsPolicyService>.Create());
            var result = await service.IsOriginAllowedAsync("InvalidOrigin");

            Assert.False(result);
        }
        
        private async Task<ConfigurationDocumentStoreHolder> GetOperationalDocumentStoreHolder_AndExecuteClientIndex()
        {
            var storeHolder = GetConfigurationDocumentStoreHolder();
            await ExecuteIndex(storeHolder.DocumentStore, new ClientIndex());
            return storeHolder;
        }
    }
}