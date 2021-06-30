using System.Linq;
using IdentityServer4.RavenDB.Storage.Entities;
using Raven.Client.Documents.Indexes;

namespace IdentityServer4.RavenDB.Storage.Indexes
{
    internal class DeviceFlowCodeIndex : AbstractIndexCreationTask<DeviceFlowCode, DeviceFlowCodeIndex.Result>
    {
        public class Result
        {
            public string UserCode { get; set; }
            public string DeviceCode { get; set; }
        }
        
        public DeviceFlowCodeIndex()
        {
            Map = deviceFlowCodes => from deviceFlowCode in deviceFlowCodes
                select new Result
                {
                    UserCode = deviceFlowCode.UserCode,
                    DeviceCode = deviceFlowCode.DeviceCode
                };
        }
    }
}