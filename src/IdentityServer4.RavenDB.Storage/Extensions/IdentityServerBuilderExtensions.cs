using IdentityServer4.RavenDB.Storage.Services;
using IdentityServer4.RavenDB.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Contrib.RavenDB.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddRavenDbConfigurationStore(
            this IIdentityServerBuilder builder)
        {
            builder.AddClientStore<ClientStore>();
            builder.AddResourceStore<ResourceStore>();
            builder.AddCorsPolicyService<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddRavenDbConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddRavenDbOperationalStore(this IIdentityServerBuilder builder)
        {
            builder.AddPersistedGrantStore<PersistedGrantStore>();
            builder.AddDeviceFlowStore<DeviceFlowStore>();
            return builder;
        }
    }
}
