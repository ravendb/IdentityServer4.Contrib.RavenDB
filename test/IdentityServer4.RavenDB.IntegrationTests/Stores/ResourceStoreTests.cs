using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Indexes;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Stores;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Stores
{
    public class ResourceStoreTests : IntegrationTest
    {
        private static IdentityResource CreateIdentityTestResource()
        {
            return new IdentityResource
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = true,
                UserClaims =
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name,
                }
            };
        }

        private static ApiResource CreateApiResourceTestResource()
        {
            return new ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Secret> { new Secret("secret".ToSha256()) },
                Scopes = { Guid.NewGuid().ToString() },
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }

        private static ApiScope CreateApiScopeTestResource()
        {
            return new ApiScope
            {
                Name = Guid.NewGuid().ToString(),
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new ApiResourceIndex().ExecuteAsync(ravenStore);

            var resource = CreateApiResourceTestResource();

            using (var session = ravenStore.OpenSession())
            {
                session.Store(resource.ToEntity());
                session.SaveChanges();
            }
            
            WaitForIndexing(ravenStore);

            ApiResource foundResource;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                foundResource = (await store.FindApiResourcesByNameAsync(new[] {resource.Name})).SingleOrDefault();
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

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new ApiResourceIndex().ExecuteAsync(ravenStore);

            var resource = CreateApiResourceTestResource();

            using (var session = ravenStore.OpenSession())
            {
                session.Store(resource.ToEntity());
                session.Store(CreateApiResourceTestResource().ToEntity());
                session.SaveChanges();
            }
            
            WaitForIndexing(ravenStore);

            ApiResource foundResource;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                foundResource = (await store.FindApiResourcesByNameAsync(new[] {resource.Name})).SingleOrDefault();
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

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourcesExist_ExpectResourcesReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new ApiResourceIndex().ExecuteAsync(ravenStore);

            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            using (var session = ravenStore.OpenSession())
            {
                session.Store(testApiResource.ToEntity());
                //session.Store(testApiScope.ToEntity());
                session.SaveChanges();
            }
            
            WaitForIndexing(ravenStore);

            IEnumerable<ApiResource> resources;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                resources = await store.FindApiResourcesByScopeNameAsync(new List<string>
                {
                    testApiScope.Name
                });
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new ApiResourceIndex().ExecuteAsync(ravenStore);

            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            using (var session = ravenStore.OpenSession())
            {
                session.Store(testIdentityResource.ToEntity());
                session.Store(testApiResource.ToEntity());
                //session.Store(testApiScope.ToEntity());
                session.Store(CreateIdentityTestResource().ToEntity());
                session.Store(CreateApiResourceTestResource().ToEntity());
                //session.Store(CreateApiScopeTestResource().ToEntity());
                session.SaveChanges();
            }
            
            WaitForIndexing(ravenStore);

            IEnumerable<ApiResource> resources;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                resources = await store.FindApiResourcesByScopeNameAsync(new[] { testApiScope.Name });
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new IdentityResourceIndex().ExecuteAsync(ravenStore);

            var resource = CreateIdentityTestResource();

            using (var session = ravenStore.OpenSession())
            {
                session.Store(resource.ToEntity());
                session.SaveChanges();
            }
            
            WaitForIndexing(ravenStore);

            IList<IdentityResource> resources;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
                {
                    resource.Name
                })).ToList();
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new IdentityResourceIndex().ExecuteAsync(ravenStore);

            var resource = CreateIdentityTestResource();

            using (var session = ravenStore.OpenSession())
            {
                session.Store(resource.ToEntity());
                session.Store(CreateIdentityTestResource().ToEntity());
                session.SaveChanges();
            }
            
            WaitForIndexing(ravenStore);

            IList<IdentityResource> resources;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
                {
                    resource.Name
                })).ToList();
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new ApiResourceIndex().ExecuteAsync(ravenStore);

            var resource = CreateApiScopeTestResource();

            using (var session = ravenStore.OpenSession())
            {
                //session.Store(resource.ToEntity());
                session.SaveChanges();
            }

            IList<ApiScope> resources;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                //resources = (await store.FindApiScopesByNameAsync(new List<string>
                //{
                //    resource.Name
                //})).ToList();
            }

            //Assert.NotNull(resources);
            //Assert.NotEmpty(resources);
            //var foundScope = resources.Single();

            //Assert.Equal(resource.Name, foundScope.Name);
            //Assert.NotNull(foundScope.UserClaims);
            //Assert.NotEmpty(foundScope.UserClaims);
        }


        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            using var ravenStore = GetDocumentStore();
            await new ApiResourceIndex().ExecuteAsync(ravenStore);
            
            var resource = CreateApiScopeTestResource();

            using (var session = ravenStore.OpenSession())
            {
                //session.Store(resource.ToEntity());
                //session.Store(CreateApiScopeTestResource().ToEntity());
                session.SaveChanges();
            }

            IList<ApiScope> resources;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                //resources = (await store.FindApiScopesByNameAsync(new List<string>
                //{
                //    resource.Name
                //})).ToList();
            }

            //Assert.NotNull(resources);
            //Assert.NotEmpty(resources);
            //Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }

        [Fact]
        public async Task GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden()
        {
            using var ravenStore = GetDocumentStore();
            await new ApiResourceIndex().ExecuteAsync(ravenStore);

            var visibleIdentityResource = CreateIdentityTestResource();
            var visibleApiResource = CreateApiResourceTestResource();
            var visibleApiScope = CreateApiScopeTestResource();
            var hiddenIdentityResource = new IdentityResource
                { Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false };
            var hiddenApiResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                Scopes = { Guid.NewGuid().ToString() },
                ShowInDiscoveryDocument = false
            };
            var hiddenApiScope = new ApiScope
            {
                Name = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = false
            };

            using (var session = ravenStore.OpenSession())
            {
                session.Store(visibleIdentityResource.ToEntity());
                session.Store(visibleApiResource.ToEntity());
                session.Store(visibleApiScope.ToEntity());

                session.Store(hiddenIdentityResource.ToEntity());
                session.Store(hiddenApiResource.ToEntity());
                session.Store(hiddenApiScope.ToEntity());

                session.SaveChanges();
            }

            WaitForIndexing(ravenStore);
            WaitForUserToContinueTheTest(ravenStore);

            Resources resources;
            using (var session = ravenStore.OpenAsyncSession())
            {
                var store = new ResourceStore(session, FakeLogger<ResourceStore>.Create());
                resources = await store.GetAllResourcesAsync();
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.NotEmpty(resources.ApiScopes);

            Assert.Contains(resources.IdentityResources, x => x.Name == visibleIdentityResource.Name);
            Assert.Contains(resources.IdentityResources, x => x.Name == hiddenIdentityResource.Name);

            Assert.Contains(resources.ApiResources, x => x.Name == visibleApiResource.Name);
            Assert.Contains(resources.ApiResources, x => x.Name == hiddenApiResource.Name);

            Assert.Contains(resources.ApiScopes, x => x.Name == visibleApiScope.Name);
            Assert.Contains(resources.ApiScopes, x => x.Name == hiddenApiScope.Name);
        }
    }
}
