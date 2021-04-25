using System;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.RavenDB.Storage.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static DocumentStoreHolderBase AddConfigurationDocumentStoreHolder(this IServiceCollection services, Action<RavenDbConfigurationStoreOptions> configureStoreOptions)
        {
            var options = RavenDbStoreOptionsHelper.GetOptions(configureStoreOptions);
            var documentStore = DocumentStoreHelper.InitializeDocumentStore(options.ConfigureDocumentStore);
            var documentStoreHolder = new ConfigurationDocumentStoreHolder(documentStore);
            
            services.AddSingleton(provider => documentStoreHolder);

            return documentStoreHolder;
        }
        
        public static DocumentStoreHolderBase AddOperationalDocumentStoreHolder(this IServiceCollection services, Action<RavenDbOperationalStoreOptions> configureStoreOptions)
        {
            var options = RavenDbStoreOptionsHelper.GetOptions(configureStoreOptions);
            var documentStore = DocumentStoreHelper.InitializeDocumentStore(options.ConfigureDocumentStore);
            var documentStoreHolder = new OperationalDocumentStoreHolder(documentStore);
            
            services.AddSingleton(provider => documentStoreHolder);

            return documentStoreHolder;
        }
    }
}