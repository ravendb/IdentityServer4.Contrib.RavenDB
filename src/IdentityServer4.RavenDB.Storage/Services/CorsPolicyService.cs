using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Entities;
using IdentityServer4.RavenDB.Storage.Indexes;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.Services
{
    internal class CorsPolicyService : ICorsPolicyService
    {
        private readonly ConfigurationDocumentStoreHolder _documentStoreHolder;
        
        public CorsPolicyService(ConfigurationDocumentStoreHolder documentStoreHolder, ILogger<CorsPolicyService> logger)
        {
            _documentStoreHolder = documentStoreHolder;
            Logger = logger;
        }

        private IAsyncDocumentSession OpenAsyncSession() => _documentStoreHolder.OpenAsyncSession();

        protected ILogger<CorsPolicyService> Logger { get; }

        /// <inheritdoc />
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            using (var session = OpenAsyncSession())
            {
                var query = session.Query<Client, ClientIndex>()
                    .Where(x => x.AllowedCorsOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase));

                var isAllowed = await query.AnyAsync();

                Logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

                return isAllowed;
            }
        }
    }
}