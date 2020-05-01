using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.Stores
{
    /// <summary>
    /// Implementation of IResourceStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        protected readonly IAsyncDocumentSession Session;

        protected readonly ILogger<ResourceStore> Logger;

        public ResourceStore(IAsyncDocumentSession session, ILogger<ResourceStore> logger)
        {
            Session = session;
            Logger = logger;
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));

            var query = Session.Query<Entities.IdentityResource>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(identityResource => identityResource.Name.In(scopeNames));

            IdentityResource[] result = (await query.ToArrayAsync()).Select(x => x.ToModel()).ToArray();

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

        /// <summary>
        /// Gets scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));

            var query = Session.Query<Entities.ApiScope>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(apiScope => apiScope.Name.In(scopeNames));

            ApiScope[] result = (await query.ToArrayAsync()).Select(x => x.ToModel()).ToArray();

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

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null) throw new ArgumentNullException(nameof(apiResourceNames));

            var query =
                Session.Query<Entities.ApiResource>()
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                    .Where(apiResource => apiResource.Name.In(apiResourceNames));

            ApiResource[] result = (await query.ToArrayAsync()).Select(x => x.ToModel()).ToArray();

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

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Resources> GetAllResourcesAsync()
        {
            var identity = Session
                .Query<Entities.IdentityResource>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)));

            var apis = Session
                .Query<Entities.ApiResource>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)));

            var scopes = Session
                .Query<Entities.ApiScope>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)));

            var result = new Resources(
                (await identity.ToArrayAsync()).Select(x => x.ToModel()),
                (await apis.ToArrayAsync()).Select(x => x.ToModel()),
                (await scopes.ToArrayAsync()).Select(x => x.ToModel())
            );

            Logger.LogDebug("Found {scopes} as all scopes, and {apis} as API resources",
                result.IdentityResources.Select(x => x.Name).Union(result.ApiScopes.Select(x => x.Name)),
                result.ApiResources.Select(x => x.Name));

            return result;
        }
    }
}
