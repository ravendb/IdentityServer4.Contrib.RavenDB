using System;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;

namespace IdentityServer4.RavenDB.Storage.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationDocumentStoreHolder(this IServiceCollection services, Action<RavenDbConfigurationStoreOptions> configureStoreOptions)
        {
            var options = RavenDbStoreOptionsHelper.GetOptions(configureStoreOptions);

            services.AddSingleton(provider => options.ResolveDocumentStoreFromServices
                ? new ConfigurationDocumentStoreHolder(provider.GetRequiredService<IDocumentStore>(), options)
                : new ConfigurationDocumentStoreHolder(options));

            return services;
        }

        public static IServiceCollection AddOperationalDocumentStoreHolder(this IServiceCollection services, Action<RavenDbOperationalStoreOptions> configureStoreOptions)
        {
            var options = RavenDbStoreOptionsHelper.GetOptions(configureStoreOptions);

            services.AddSingleton(provider => options.ResolveDocumentStoreFromServices
                ? new OperationalDocumentStoreHolder(provider.GetRequiredService<IDocumentStore>(), options)
                : new OperationalDocumentStoreHolder(options));

            return services;
        }
    }
}