using IdentityServer4.RavenDB.Storage.Stores;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Contrib.RavenDB.Extensions
{
    public static class IdentityServerBuilderExtensions
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
