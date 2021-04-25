using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Indexes;
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
    internal class ClientStore : IClientStore
    {
        private readonly ConfigurationDocumentStoreHolder _documentStoreHolder;
        
        public ClientStore(ConfigurationDocumentStoreHolder documentStoreHolder, ILogger<ClientStore> logger)
        {
            _documentStoreHolder = documentStoreHolder;
            Logger = logger;
        }

        private IAsyncDocumentSession OpenAsyncSession() => _documentStoreHolder.OpenAsyncSession();

        protected ILogger<ClientStore> Logger { get; }

        /// <inheritdoc />
        public virtual async Task<Client> FindClientByIdAsync(string clientId)
        {
            using (var session = OpenAsyncSession())
            {
                var client = await session.Query<Entities.Client, ClientIndex>()
                    .Where(x => x.ClientId == clientId)
                    .SingleOrDefaultAsync();

                if (client == null) return null;

                var model = client.ToModel();

                Logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, model != null);

                return model;
            }
        }
    }
}
