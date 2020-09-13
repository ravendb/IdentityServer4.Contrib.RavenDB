using System.Linq;
using IdentityServer4.RavenDB.Storage.Entities;
using Raven.Client.Documents.Indexes;

namespace IdentityServer4.RavenDB.Storage.Indexes
{
    public class IdentityResourceIndex : AbstractIndexCreationTask<IdentityResource, IdentityResourceIndex.Result>
    {
        public class Result
        {
            public string Name { get; set; }
        }
        
        public IdentityResourceIndex()
        {
            Map = apiResources => from apiResource in apiResources
                select new Result
                {
                    Name = apiResource.Name
                };
        }
    }
}