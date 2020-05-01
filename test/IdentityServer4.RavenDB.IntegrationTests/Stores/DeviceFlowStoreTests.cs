using System;
using System.Threading.Tasks;
using IdentityServer4.RavenDB.Storage.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Stores
{
    public class DeviceFlowStoreTests : IntegrationTest
    {

        [Fact]
        public async Task RemoveByDeviceCodeAsync_WhenDeviceCodeDoesNotExists_ExpectSuccess()
        {
            using (var ravenStore = GetDocumentStore())
            {
                using (var session = ravenStore.OpenAsyncSession())
                {
                    var store = new DeviceFlowStore(session, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());

                    await store.RemoveByDeviceCodeAsync($"device_{Guid.NewGuid().ToString()}");
                }
            }
        }
    }
}
