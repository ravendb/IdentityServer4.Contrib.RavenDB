using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IdentityServer4.RavenDB.Storage.Entities;
using Raven.Client.Documents.Indexes;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Indexes
{
    internal class ApiResourceIndex : AbstractIndexCreationTask<ApiResource, ApiResourceIndex.Result>
    {
        public class Result
        {
            public string Name { get; set; }
            public List<string> Scopes { get; set; }
        }
        
        public ApiResourceIndex()
        {
            Map = apiResources => from apiResource in apiResources
                select new Result
                {
                    Name = apiResource.Name,
                    Scopes = apiResource.Scopes
                };
        }
    }
}