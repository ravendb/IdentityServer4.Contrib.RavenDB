using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Indexes;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using ApiResource = IdentityServer4.Models.ApiResource;
using IdentityResource = IdentityServer4.Models.IdentityResource;

namespace IdentityServer4.RavenDB.Storage.Stores
{
    /// <summary>
    /// Implementation of IResourceStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        protected IAsyncDocumentSession Session { get; }
        protected ILogger<ResourceStore> Logger { get; }

        public ResourceStore(IAsyncDocumentSession session, ILogger<ResourceStore> logger)
        {
            Session = session;
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));

            var identityResources = await Session.Query<Entities.IdentityResource, IdentityResourceIndex>()
                .Where(x => x.Name.In(scopeNames))
                .ToArrayAsync();

            IdentityResource[] result = identityResources.Select(x => x.ToModel()).ToArray();

            if (result.Any())
            {
                Logger.LogDebug("Found {scopes} identity scopes in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {scopes} identity scopes in database", scopeNames);
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));

            var scopes = await Session.Query<Entities.ApiScope, ApiScopeIndex>()
                .Where(x => x.Name.In(scopeNames))
                .ToArrayAsync();

            ApiScope[] result = scopes.Select(x => x.ToModel()).ToArray();

            if (result.Any())
            {
                Logger.LogDebug("Found {scopes} scopes in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {scopes} scopes in database", scopeNames);
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));

            var apiResources = await Session.Query<Entities.ApiResource, ApiResourceIndex>()
                    .Where(apiResource => apiResource.Scopes.ContainsAny(scopeNames))
                    .ToArrayAsync();

            var result = apiResources.Select(x => x.ToModel()).ToArray();

            if (result.Any())
            {
                Logger.LogDebug("Found {apis} API resource in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {apis} API resource in database", scopeNames);
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null) throw new ArgumentNullException(nameof(apiResourceNames));

            var apiResources = await Session.Query<Entities.ApiResource, ApiResourceIndex>()
                .Where(x => x.Name.In(apiResourceNames))
                .ToArrayAsync();

            ApiResource[] result = apiResources.Select(x => x.ToModel()).ToArray();

            if (result.Any())
            {
                Logger.LogDebug("Found {apis} API resource in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {apis} API resource in database", apiResourceNames);
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<Resources> GetAllResourcesAsync()
        {
            var identity = Session.Query<Entities.IdentityResource>();

            var apiResources = Session.Query<Entities.ApiResource>();

            var apiScopes = Session.Query<Entities.ApiScope>();

            var result = new Resources(
                (await identity.ToArrayAsync()).Select(x => x.ToModel()),
                (await apiResources.ToArrayAsync()).Select(x => x.ToModel()),
                    (await apiScopes.ToArrayAsync()).Select(x => x.ToModel())
            );

            Logger.LogDebug("Found {scopes} as all scopes, and {apiResources} as API resources, and {apiScopes} as API scopes",
                result.IdentityResources.Select(x => x.Name),
                result.ApiResources.Select(x => x.Name),
                result.ApiScopes.Select(x => x.Name));

            return result;
        }
    }
}
