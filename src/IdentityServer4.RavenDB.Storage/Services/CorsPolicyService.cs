using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.RavenDB.Storage.Entities;
using IdentityServer4.RavenDB.Storage.Indexes;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        public CorsPolicyService(IAsyncDocumentSession session, ILogger<CorsPolicyService> logger)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
            Logger = logger;
        }

        protected IAsyncDocumentSession Session { get; }
        protected ILogger<CorsPolicyService> Logger { get; }

        /// <inheritdoc />
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var query = Session.Query<Client, ClientIndex>()
                .Where(x => x.AllowedCorsOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase));

            var isAllowed = await query.AnyAsync();

            Logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return isAllowed;
        }
    }
}