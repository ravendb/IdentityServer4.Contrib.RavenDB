using System;
using System.Linq;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Options;
using IdentityServer4.RavenDB.Storage.Services;
using IdentityServer4.RavenDB.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.RavenDB.Storage.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddRavenDbConfigurationStore(
            this IIdentityServerBuilder builder, Action<RavenDbConfigurationStoreOptions> configureStoreOptions)
        {
            builder.Services.AddConfigurationDocumentStoreHolder(configureStoreOptions);
            
            builder.AddClientStore<ClientStore>();
            builder.AddResourceStore<ResourceStore>();
            builder.AddCorsPolicyService<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddRavenDbConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            CheckAddConfigurationStoreHolderWasCalled(builder.Services);
            
            builder.AddInMemoryCaching();

            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddRavenDbOperationalStore(this IIdentityServerBuilder builder, Action<RavenDbOperationalStoreOptions> configureStoreOptions)
        {
            builder.Services.AddOperationalDocumentStoreHolder(configureStoreOptions);
            
            builder.AddPersistedGrantStore<PersistedGrantStore>();
            builder.AddDeviceFlowStore<DeviceFlowStore>();
            return builder;
        }

        private static void CheckAddConfigurationStoreHolderWasCalled(IServiceCollection services)
        {
            if (services.Any(x => x.ServiceType == typeof(ConfigurationDocumentStoreHolder)) == false)
            {
                throw new InvalidOperationException($"{nameof(AddRavenDbConfigurationStore)} extension method must be called before {nameof(AddRavenDbConfigurationStoreCache)} can be configured.");
            }
        }
    }
}
