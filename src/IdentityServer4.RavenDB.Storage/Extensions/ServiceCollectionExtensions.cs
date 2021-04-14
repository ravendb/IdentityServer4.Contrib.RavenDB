using System;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Options;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.RavenDB.Storage.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static DocumentStoreHolderBase AddConfigurationDocumentStoreHolder(this IServiceCollection services, Action<RavenDbConfigurationStoreOptions> configurationStoreOptionsAction)
        {
            var options = RavenDbStoreOptionsHelper.GetRavenDbStoreOptions(configurationStoreOptionsAction);
            var documentStore = DocumentStoreHelper.InitializeDocumentStore(options.ConfigureDocumentStore);
            var documentStoreHolder = new ConfigurationDocumentStoreHolder(documentStore);
            
            services.AddSingleton<IConfigurationDocumentStoreHolder>(provider => documentStoreHolder);

            return documentStoreHolder;
        }
        
        public static DocumentStoreHolderBase AddOperationalDocumentStoreHolder(this IServiceCollection services, Action<RavenDbOperationalStoreOptions> operationalStoreOptionsAction)
        {
            var options = RavenDbStoreOptionsHelper.GetRavenDbStoreOptions(operationalStoreOptionsAction);
            var documentStore = DocumentStoreHelper.InitializeDocumentStore(options.ConfigureDocumentStore);
            var documentStoreHolder = new OperationalDocumentStoreHolder(documentStore);
            
            services.AddSingleton<IOperationalDocumentStoreHolder>(provider => documentStoreHolder);

            return documentStoreHolder;
        }
    }
}