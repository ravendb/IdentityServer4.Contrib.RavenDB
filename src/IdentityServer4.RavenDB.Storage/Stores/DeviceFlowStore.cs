using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.DocumentStoreHolder;
using IdentityServer4.RavenDB.Storage.Entities;
using IdentityServer4.RavenDB.Storage.Extensions;
using IdentityServer4.RavenDB.Storage.Indexes;
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
    internal class DeviceFlowStore : IDeviceFlowStore
    {
        private readonly OperationalDocumentStoreHolder _documentStoreHolder;
        
        public DeviceFlowStore(
            OperationalDocumentStoreHolder documentStoreHolder,
            IPersistentGrantSerializer serializer,
            ILogger<DeviceFlowStore> logger)
        {
            _documentStoreHolder = documentStoreHolder;
            Serializer = serializer;
            Logger = logger;
        }

        private IAsyncDocumentSession OpenAsyncSession() => _documentStoreHolder.OpenAsyncSession();

        protected ILogger<DeviceFlowStore> Logger { get; }
        protected IPersistentGrantSerializer Serializer { get; }

        /// <inheritdoc />
        public virtual async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            var device = await FindByDeviceCodeAsync(deviceCode);
            if (device != null)
                throw new Exception($"device code {deviceCode} is already registered");

            device = await FindByUserCodeAsync(userCode);
            if (device != null)
                throw new Exception($"user code {userCode} is already registered");

            using (var session = OpenAsyncSession())
            {
                await session.StoreAsync(ToEntity(data, deviceCode, userCode));

                await session.WaitForIndexAndSaveChangesAsync<DeviceFlowCodeIndex>();
            }
        }

        /// <inheritdoc />
        public virtual async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            using (var session = OpenAsyncSession())
            {
                var deviceFlowCodes = await session.Query<DeviceFlowCode, DeviceFlowCodeIndex>()
                    .FirstOrDefaultAsync(x => x.UserCode == userCode);

                var model = ToModel(deviceFlowCodes?.Data);

                Logger.LogDebug("{userCode} found in database: {userCodeFound}", userCode, model != null);

                return model;
            }
        }

        /// <inheritdoc />
        public virtual async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            using (var session = OpenAsyncSession())
            {
                var deviceFlowCodes = await session.Query<DeviceFlowCode, DeviceFlowCodeIndex>()
                    .FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

                var model = ToModel(deviceFlowCodes?.Data);

                Logger.LogDebug("{deviceCode} found in database: {deviceCodeFound}", deviceCode, model != null);

                return model;
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            using (var session = OpenAsyncSession())
            {
                var existing = await session.Query<DeviceFlowCode, DeviceFlowCodeIndex>()
                    .SingleOrDefaultAsync(x => x.UserCode == userCode);

                if (existing == null)
                {
                    Logger.LogError("{userCode} not found in database", userCode);
                    throw new InvalidOperationException("Could not update device code");
                }

                var entity = ToEntity(data, existing.DeviceCode, userCode);
                Logger.LogDebug("{userCode} found in database", userCode);

                existing.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value;
                existing.Data = entity.Data;

                try
                {
                    await session.WaitForIndexAndSaveChangesAsync<DeviceFlowCodeIndex>();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("exception updating {userCode} user code in database: {error}", userCode, ex.Message);
                }
            }
        }

        /// <inheritdoc />
        public virtual async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            using (var session = OpenAsyncSession())
            {
                var deviceFlowCode = await session.Query<DeviceFlowCode, DeviceFlowCodeIndex>()
                    .FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

                if (deviceFlowCode != null)
                {
                    Logger.LogDebug("removing {deviceCode} device code from database", deviceCode);

                    session.Delete(deviceFlowCode);

                    try
                    {
                        await session.WaitForIndexAndSaveChangesAsync<DeviceFlowCodeIndex>();
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

        protected DeviceFlowCode ToEntity(DeviceCode model, string deviceCode, string userCode)
        {
            if (model == null || deviceCode == null || userCode == null) return null;

            return new DeviceFlowCode
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                ClientId = model.ClientId,
                SubjectId = model.Subject?.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = model.CreationTime,
                Expiration = model.CreationTime.AddSeconds(model.Lifetime),
                Data = Serializer.Serialize(model)
            };
        }

        /// <summary>
        /// Converts a serialized DeviceCode to a model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected DeviceCode ToModel(string entity)
        {
            if (entity == null) return null;

            return Serializer.Deserialize<DeviceCode>(entity);
        }
    }
}
