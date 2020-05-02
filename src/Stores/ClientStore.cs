using System;
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
    /// Implementation of IClientStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IClientStore" />
    public class ClientStore : IClientStore
    {
        protected readonly IAsyncDocumentSession Session;

        protected readonly ILogger<ClientStore> Logger;

        public ClientStore(IAsyncDocumentSession session, ILogger<ClientStore> logger)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
            Logger = logger;
        }

        public virtual async Task<Client> FindClientByIdAsync(string clientId)
        {
            var baseQuery = Session.Query<Entities.Client>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(x => x.ClientId == clientId)
                .Take(1);

            var client = await baseQuery.FirstOrDefaultAsync();
            if (client == null) return null;

            var model = client.ToModel();

            Logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, model != null);

            return model;
        }
    }
}
