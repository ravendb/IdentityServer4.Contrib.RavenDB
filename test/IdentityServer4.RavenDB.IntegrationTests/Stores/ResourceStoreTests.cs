using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Indexes;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Stores;
using Raven.Client.Documents.Indexes;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Stores
{
    public class ResourceStoreTests : IntegrationTestBase
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
                ApiSecrets = new List<Secret> { new Secret(CryptographyHelper.CreateHash("secret")) },
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
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new ApiResourceIndex());

            var resource = CreateApiResourceTestResource();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(resource.ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var apiResourceNames = new[]
            {
                resource.Name,
                "non-existent"
            };
            
            var foundResource = (await store.FindApiResourcesByNameAsync(apiResourceNames)).SingleOrDefault();
            
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
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new ApiResourceIndex());

            var resource = CreateApiResourceTestResource();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(resource.ToEntity());
               await session.StoreAsync(CreateApiResourceTestResource().ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var foundResource = (await store.FindApiResourcesByNameAsync(new[] {resource.Name})).SingleOrDefault();
            
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
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new ApiResourceIndex());

            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(testApiResource.ToEntity());
                //session.Store(testApiScope.ToEntity());
                await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var resources = await store.FindApiResourcesByScopeNameAsync(new List<string>
            {
                testApiScope.Name
            });
            
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new ApiResourceIndex());

            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(testIdentityResource.ToEntity());
               await session.StoreAsync(testApiResource.ToEntity());
                //session.Store(testApiScope.ToEntity());
               await session.StoreAsync(CreateIdentityTestResource().ToEntity());
               await session.StoreAsync(CreateApiResourceTestResource().ToEntity());
                //session.Store(CreateApiScopeTestResource().ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var resources = await store.FindApiResourcesByScopeNameAsync(new[] { testApiScope.Name });
            
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new IdentityResourceIndex());

            var resource = CreateIdentityTestResource();

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(resource.ToEntity());
                await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
            {
                resource.Name,
                "non-existent"
            })).ToList();
            

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
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new IdentityResourceIndex());

            var resource = CreateIdentityTestResource();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(resource.ToEntity());
               await session.StoreAsync(CreateIdentityTestResource().ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
            {
                resource.Name
            })).ToList();
            
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }


        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new ApiScopeIndex());

            var resource = CreateApiScopeTestResource();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(resource.ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var resources = (await store.FindApiScopesByNameAsync(new List<string>
            {
                resource.Name,
                "non-existent"
            })).ToList();
            
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new ApiScopeIndex());
            
            var resource = CreateApiScopeTestResource();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(resource.ToEntity());
               await session.StoreAsync(CreateApiScopeTestResource().ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var resources = (await store.FindApiScopesByNameAsync(new List<string>
            {
                resource.Name,
                "non-existent"
            })).ToList();
            
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }

        [Fact]
        public async Task GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden()
        {
            var storeHolder = await GetConfigurationDocumentStoreHolder_AndExecuteIndex(new ApiResourceIndex());

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

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(visibleIdentityResource.ToEntity());
               await session.StoreAsync(visibleApiResource.ToEntity());
               await session.StoreAsync(visibleApiScope.ToEntity());

               await session.StoreAsync(hiddenIdentityResource.ToEntity());
               await session.StoreAsync(hiddenApiResource.ToEntity());
               await session.StoreAsync(hiddenApiScope.ToEntity());

               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.DocumentStore);
            WaitForUserToContinueTheTest(storeHolder.DocumentStore);

            var store = new ResourceStore(storeHolder, FakeLogger<ResourceStore>.Create());
            var resources = await store.GetAllResourcesAsync();
            
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
        
        private async Task<ConfigurationDocumentStoreHolder> GetConfigurationDocumentStoreHolder_AndExecuteIndex(AbstractIndexCreationTask index)
        {
            var storeHolder = GetConfigurationDocumentStoreHolder();
            await ExecuteIndex(storeHolder.DocumentStore, index);
            return storeHolder;
        }
    }
}
