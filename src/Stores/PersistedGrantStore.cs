using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.RavenDB.Stores
{
    /// <summary>
    /// Implementation of IPersistedGrantStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IPersistedGrantStore" />
    public class PersistedGrantStore : IPersistedGrantStore
    {
        public Task StoreAsync(PersistedGrant grant)
        {
            throw new NotImplementedException();
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            throw new NotImplementedException();
        }
    }
}
