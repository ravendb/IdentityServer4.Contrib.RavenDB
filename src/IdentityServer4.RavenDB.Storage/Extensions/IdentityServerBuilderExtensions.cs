using System;
using System.Linq;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using IdentityServer4.RavenDB.Storage.Services;
using IdentityServer4.RavenDB.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.RavenDB.Storage.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddRavenDbConfigurationStore(
            this IIdentityServerBuilder builder, Action<RavenDbConfigurationStoreOptions> configurationStoreOptionsAction)
        {
            var storeHolder = builder.Services.AddConfigurationDocumentStoreHolder(configurationStoreOptionsAction);
            
            IndexHelper.ExecuteIndexes(storeHolder.DocumentStore, IndexHelper.ConfigurationStoreIndexes);
            
            builder.AddClientStore<ClientStore>();
            builder.AddResourceStore<ResourceStore>();
            builder.AddCorsPolicyService<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddRavenDbConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            var services = builder.Services;
            
            if (services.Any(x => x.ServiceType == typeof(IConfigurationDocumentStoreHolder) == false))
            {
                throw new InvalidOperationException($"{nameof(AddRavenDbConfigurationStore)} extension method must be called before {nameof(AddRavenDbConfigurationStoreCache)} can be configured.");
            }
            
            builder.AddInMemoryCaching();

            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddRavenDbOperationalStore(this IIdentityServerBuilder builder, Action<RavenDbOperationalStoreOptions> operationalStoreOptionsAction)
        {
            var storeHolder = builder.Services.AddOperationalDocumentStoreHolder(operationalStoreOptionsAction);
            
            IndexHelper.ExecuteIndexes(storeHolder.DocumentStore, IndexHelper.OperationalStoreIndexes);
            
            builder.AddPersistedGrantStore<PersistedGrantStore>();
            builder.AddDeviceFlowStore<DeviceFlowStore>();
            return builder;
        }
    }
}
