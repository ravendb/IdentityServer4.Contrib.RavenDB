using System;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.RavenDB.Storage.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddConfigurationDocumentStoreHolder(this IServiceCollection services, Action<RavenDbConfigurationStoreOptions> configureStoreOptions)
        {
            var options = RavenDbStoreOptionsHelper.GetOptions(configureStoreOptions);
            
            services.AddSingleton(provider => new ConfigurationDocumentStoreHolder(options));
        }
        
        public static void AddOperationalDocumentStoreHolder(this IServiceCollection services, Action<RavenDbOperationalStoreOptions> configureStoreOptions)
        {
            var options = RavenDbStoreOptionsHelper.GetOptions(configureStoreOptions);
            
            services.AddSingleton(provider => new OperationalDocumentStoreHolder(options));
        }
    }
}