using System;
using IdentityServer4.Configuration;
using IdentityServer4.RavenDB.Storage.Extensions;
using IdentityServer4.RavenDB.Storage.Services;
using IdentityServer4.RavenDB.Storage.Stores;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.ExtensionsTests
{
    public class IdentityServerBuilderExtensionTests : IntegrationTestBase
    {
        [Fact]
        public void AddRavenDbConfigurationStoreCache_Throws_When_AddRavenDbConfigurationStore_IsNotConfiguredFirst()
        {
            var builder = new IdentityServerBuilder(new ServiceCollection());
            Assert.Throws<InvalidOperationException>(() => builder.AddRavenDbConfigurationStoreCache());
        }

        [Fact]
        public void
            AddRavenDbConfigurationStoreCache_IsConfiguredCorrectly_When_AddRavenDbConfigurationStore_IsCalledFirst()
        {
            var documentStore = GetDocumentStore();
            var builder = new IdentityServerBuilder(new ServiceCollection());
            
            builder.AddRavenDbConfigurationStore(options =>
            {
                options.ConfigureDocumentStore = store =>
                {
                    store.Database = documentStore.Database;
                    store.Urls = documentStore.Urls;
                };
            })
                .AddRavenDbConfigurationStoreCache();

            var services = builder.Services;

            AssertServiceAdded<CachingResourceStore<ResourceStore>>(services);
            AssertServiceAdded<CachingClientStore<ValidatingClientStore<ClientStore>>>(services);
            AssertServiceAdded<CachingCorsPolicyService<CorsPolicyService>>(services);
        }

        private void AssertServiceAdded<T>(IServiceCollection services)
        {
            Assert.Contains(services, descriptor => descriptor.ImplementationType == typeof(T));
        }
    }
}