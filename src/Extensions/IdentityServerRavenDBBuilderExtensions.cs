using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.RavenDB.Storage.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.RavenDB.Storage.Extensions
{
    public static class IdentityServerRavenDBBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            //builder.Services.Configure<RavenDBConfiguration>(configuration);

            return builder.AddConfigurationStore();
        }

        private static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            //builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }
    }
}
