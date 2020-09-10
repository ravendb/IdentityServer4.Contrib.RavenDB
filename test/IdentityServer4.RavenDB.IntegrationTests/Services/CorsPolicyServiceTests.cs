using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Services;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Services
{
    public class CorsPolicyServiceTests : IntegrationTest
    {
        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue()
        {
            const string testCorsOrigin = "https://identityserver.io/";

            using var store = GetDocumentStore();

            using (var session = store.OpenAsyncSession())
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

            bool result;

            using (var session = store.OpenAsyncSession())
            {
                var service = new CorsPolicyService(session, FakeLogger<CorsPolicyService>.Create());
                result = await service.IsOriginAllowedAsync(testCorsOrigin.ToUpperInvariant());
            }

            Assert.True(result);
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse()
        {
            using var store = GetDocumentStore();

            using (var session = store.OpenAsyncSession())
            {
                await session.StoreAsync(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
                }.ToEntity());

                await session.SaveChangesAsync();
            }

            bool result;
            using (var session = store.OpenAsyncSession())
            {
                var service = new CorsPolicyService(session, FakeLogger<CorsPolicyService>.Create());
                result = await service.IsOriginAllowedAsync("InvalidOrigin");
            }

            Assert.False(result);
        }
    }
}