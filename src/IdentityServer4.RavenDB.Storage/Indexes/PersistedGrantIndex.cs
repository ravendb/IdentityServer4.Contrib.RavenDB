using System.Linq;
using IdentityServer4.RavenDB.Storage.Entities;
using Raven.Client.Documents.Indexes;

namespace IdentityServer4.RavenDB.Storage.Indexes
{
    internal class PersistedGrantIndex : AbstractIndexCreationTask<PersistedGrant, PersistedGrantIndex.Result>
    {
        public class Result
        {
            public string Key { get; set; }
            public string ClientId { get; set; }
            public string SessionId { get; set; }
            public string SubjectId { get; set; }
            public string Type { get; set; }
        }
        
        public PersistedGrantIndex()
        {
            Map = grants => from grant in grants
                select new Result
                {
                    Key = grant.Key,
                    ClientId = grant.ClientId,
                    SessionId = grant.SessionId,
                    SubjectId = grant.SubjectId,
                    Type = grant.Type
                };
        }
    }
}