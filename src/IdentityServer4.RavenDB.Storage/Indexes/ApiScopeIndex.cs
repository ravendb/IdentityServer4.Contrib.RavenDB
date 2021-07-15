using System.Linq;
using IdentityServer4.RavenDB.Storage.Entities;
using Raven.Client.Documents.Indexes;

namespace IdentityServer4.RavenDB.Storage.Indexes
{
    internal class ApiScopeIndex : AbstractIndexCreationTask<ApiScope, ApiScopeIndex.Result>
    {
        public class Result
        {
            public string Name { get; set; }
        }

        public ApiScopeIndex()
        {
            Map = apiScopes => from apiScope in apiScopes
                select new Result
                {
                    Name = apiScope.Name
                };
        }
    }
}