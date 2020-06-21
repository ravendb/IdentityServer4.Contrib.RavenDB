using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.Stores
{
    /// <summary>
    /// Implementation of IPersistedGrantStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IPersistedGrantStore" />
    public class PersistedGrantStore : IPersistedGrantStore
    {
        protected readonly IAsyncDocumentSession Session;

        protected readonly ILogger<PersistedGrantStore> Logger;

        public PersistedGrantStore(IAsyncDocumentSession session, ILogger<PersistedGrantStore> logger)
        {
            Session = session;
            Logger = logger;
        }

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

        public virtual async Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = await Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefaultAsync(x => x.Key == key);
            
            var model = persistedGrant?.ToModel();

            Logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, model != null);

            return model;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var persistedGrants = await Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(x => x.SubjectId == subjectId)
                .ToListAsync();

            var model = persistedGrants.Select(x => x.ToModel());

            Logger.LogDebug("{persistedGrantCount} persisted grants found for {subjectId}", persistedGrants.Count, subjectId);

            return model;
        }

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
                    Logger.LogInformation("exception removing {persistedGrantKey} persisted grant from database: {error}", key, ex.Message);
                }
            }
            else
            {
                Logger.LogDebug("no {persistedGrantKey} persisted grant found in database", key);
            }
        }

        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            throw new NotImplementedException();
        }

        public virtual async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var persistedGrants = await Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(x => x.SubjectId == subjectId && x.ClientId == clientId)
                .ToListAsync();

            Logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}", persistedGrants.Count, subjectId, clientId);

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
                Logger.LogInformation("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}: {error}", persistedGrants.Count, subjectId, clientId, ex.Message);
            }
        }

        public virtual async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var persistedGrants = await Session.Query<Entities.PersistedGrant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(x => x.SubjectId == subjectId && x.ClientId == clientId)
                .ToListAsync();

            Logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}", persistedGrants.Count, subjectId, clientId);

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
                Logger.LogInformation("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}: {error}", persistedGrants.Count, subjectId, clientId, ex.Message);
            }
        }
    }
}
