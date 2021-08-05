using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Services;
using Xunit;

namespace IdentityServer4.RavenDB.Storage.Tests.ServicesTests
{
    public class CorsPolicyServiceTests : IntegrationTestBase
    {
        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue()
        {
            const string testCorsOrigin = "https://identityserver.io/";

            var storeHolder = GetConfigurationDocumentStoreHolder();

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
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var service = new CorsPolicyService(storeHolder, FakeLogger<CorsPolicyService>.Create());
            var result = await service.IsOriginAllowedAsync(testCorsOrigin.ToUpperInvariant());
            
            Assert.True(result);
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse()
        {
            var storeHolder = GetConfigurationDocumentStoreHolder();

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

            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var service = new CorsPolicyService(storeHolder, FakeLogger<CorsPolicyService>.Create());
            var result = await service.IsOriginAllowedAsync("InvalidOrigin");

            Assert.False(result);
        }
    }
}