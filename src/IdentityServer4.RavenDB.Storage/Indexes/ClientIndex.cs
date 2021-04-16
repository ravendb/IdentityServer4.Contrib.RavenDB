using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IdentityServer4.RavenDB.Storage.Entities;
using Raven.Client.Documents.Indexes;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Indexes
{
    internal class ClientIndex : AbstractIndexCreationTask<Client, ClientIndex.Result>
    {
        public class Result
        {
            public string ClientId { get; set; }
            public List<string> AllowedCorsOrigins { get; set; }
        }
        
        public ClientIndex()
        {
            Map = clients => from client in clients
                select new Result
                {
                    ClientId = client.ClientId,
                    AllowedCorsOrigins = client.AllowedCorsOrigins
                };
        }
    }
}