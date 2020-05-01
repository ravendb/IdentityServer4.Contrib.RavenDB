using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.RavenDB.Storage.Stores
{
    /// <summary>
    /// Implementation of IDeviceFlowStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IDeviceFlowStore" />
    public class DeviceFlowStore : IDeviceFlowStore
    {
        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            throw new NotImplementedException();
        }

        public Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            throw new NotImplementedException();
        }

        public Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            throw new NotImplementedException();
        }
    }
}
