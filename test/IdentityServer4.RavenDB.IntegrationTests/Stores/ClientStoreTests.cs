using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Stores;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Stores
{
    public class ClientStoreTests : IntegrationTest
    {
        private static ApiResource CreateApiResourceTestResource()
        {
            return new ApiResource()
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Secret> { new Secret("secret".ToSha256()) },
                Scopes = { Guid.NewGuid().ToString() },
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                }
            };
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            using (var ravenStore = GetDocumentStore())
            {
                var resource = CreateApiResourceTestResource();

                using (var session = ravenStore.OpenSession())
                {
                    session.Store(resource.ToEntity());
                    session.SaveChanges();
                }

                WaitForUserToContinueTheTest(ravenStore);

                ApiResource foundResource;
                using (var session = ravenStore.OpenAsyncSession())
                {
                    var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                    foundResource = (await store.FindApiResourcesByNameAsync(new[] { resource.Name })).SingleOrDefault();
                }

                Assert.NotNull(foundResource);
                Assert.True(foundResource.Name == resource.Name);

                Assert.NotNull(foundResource.UserClaims);
                Assert.NotEmpty(foundResource.UserClaims);
                Assert.NotNull(foundResource.ApiSecrets);
                Assert.NotEmpty(foundResource.ApiSecrets);
                Assert.NotNull(foundResource.Scopes);
                Assert.NotEmpty(foundResource.Scopes);
            }
        }
    }
}
