using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
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
    /// Implementation of IPersistedGrantStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IPersistedGrantStore" />
    public class PersistedGrantStore : IPersistedGrantStore
    {
        public PersistedGrantStore(IAsyncDocumentSession session, ILogger<PersistedGrantStore> logger)
        {
            Session = session;
            Logger = logger;
        }

        protected IAsyncDocumentSession Session { get; }
        protected ILogger<PersistedGrantStore> Logger { get; }

        /// <inheritdoc />
        public virtual async Task StoreAsync(PersistedGrant token)
        {
            var existing = await Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .SingleOrDefaultAsync(x => x.Key == token.Key);
            if (existing == null)
            {
                Logger.LogDebug("{persistedGrantKey} not found in database", token.Key);

                var persistedGrant = token.ToEntity();
                await Session.StoreAsync(persistedGrant);
            }
            else
            {
                Logger.LogDebug("{persistedGrantKey} found in database", token.Key);

                token.UpdateEntity(existing);
            }

            try
            {
                await Session.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("exception updating {persistedGrantKey} persisted grant in database: {error}", token.Key, ex.Message);
            }
        }

        /// <inheritdoc />
        public virtual async Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = await Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefaultAsync(x => x.Key == key);

            var model = persistedGrant?.ToModel();

            Logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, model != null);

            return model;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var persistedGrants = await Filter(filter).ToListAsync();

            var model = persistedGrants.Select(x => x.ToModel());

            Logger.LogDebug("{persistedGrantCount} persisted grants", persistedGrants.Count);

            return model;
        }

        /// <inheritdoc />
        public virtual async Task RemoveAsync(string key)
        {
            var persistedGrant = await Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefaultAsync(x => x.Key == key);

            if (persistedGrant != null)
            {
                Logger.LogDebug("removing {persistedGrantKey} persisted grant from database", key);

                Session.Delete(persistedGrant);

                try
                {
                    await Session.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError("exception removing {persistedGrantKey} persisted grant from database: {error}", key, ex.Message);
                }
            }
            else
            {
                Logger.LogDebug("no {persistedGrantKey} persisted grant found in database", key);
            }
        }

        /// <inheritdoc />
        public virtual async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var persistedGrants = await Filter(filter).ToListAsync();

            Logger.LogDebug("removing {persistedGrantCount} persisted grants from database", persistedGrants.Count);

            foreach (var persistedGrant in persistedGrants)
            {
                Session.Delete(persistedGrant);
            }

            try
            {
                await Session.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError("removing {persistedGrantCount} persisted grants from database: {error}", persistedGrants.Count, ex.Message);
            }
        }

        private IRavenQueryable<Entities.PersistedGrant> Filter(PersistedGrantFilter filter)
        {
            var query = Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)));

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                query = query.Where(x => x.ClientId == filter.ClientId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                query = query.Where(x => x.SessionId == filter.SessionId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(x => x.Type == filter.Type);
            }

            return query;
        }
    }
}
