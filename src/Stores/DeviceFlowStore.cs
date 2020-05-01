using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Entities;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.Stores
{
    /// <summary>
    /// Implementation of IDeviceFlowStore that uses RavenDB.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IDeviceFlowStore" />
    public class DeviceFlowStore : IDeviceFlowStore
    {
        protected readonly IAsyncDocumentSession Session;

        protected readonly ILogger<DeviceFlowStore> Logger;

        protected readonly IPersistentGrantSerializer Serializer;

        public DeviceFlowStore(
            IAsyncDocumentSession session,
            IPersistentGrantSerializer serializer,
            ILogger<DeviceFlowStore> logger)
        {
            Session = session;
            Serializer = serializer;
            Logger = logger;
        }


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

        public virtual async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            var deviceFlowCode = await Session.Query<DeviceFlowCodes>()
                .FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

            if (deviceFlowCode != null)
            {
                Logger.LogDebug("removing {deviceCode} device code from database", deviceCode);

                Session.Delete(deviceFlowCode);

                try
                {
                    await Session.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation("exception removing {deviceCode} device code from database: {error}", deviceCode, ex.Message);
                }
            }
            else
            {
                Logger.LogDebug("no {deviceCode} device code found in database", deviceCode);
            }
        }
    }
}
