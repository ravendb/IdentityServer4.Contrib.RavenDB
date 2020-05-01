using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Entities;
using IdentityServer4.RavenDB.Storage.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Stores
{
    public class DeviceFlowStoreTests : IntegrationTest
    {
        private readonly IPersistentGrantSerializer serializer = new PersistentGrantSerializer();

        [Fact]
        public async Task RemoveByDeviceCodeAsync_WhenDeviceCodeExists_ExpectDeviceCodeDeleted()
        {
            using (var ravenStore = GetDocumentStore())
            {
                var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
                var testUserCode = $"user_{Guid.NewGuid().ToString()}";

                var existingDeviceCode = new DeviceCode
                {
                    ClientId = "device_flow",
                    RequestedScopes = new[] {"openid", "api1"},
                    CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                    Lifetime = 300,
                    IsOpenId = true
                };

                using (var session = ravenStore.OpenSession())
                {
                    session.Store(new DeviceFlowCodes
                    {
                        DeviceCode = testDeviceCode,
                        UserCode = testUserCode,
                        ClientId = existingDeviceCode.ClientId,
                        CreationTime = existingDeviceCode.CreationTime,
                        Expiration = existingDeviceCode.CreationTime.AddSeconds(existingDeviceCode.Lifetime),
                        Data = serializer.Serialize(existingDeviceCode)
                    });
                    session.SaveChanges();
                }

                using (var session = ravenStore.OpenAsyncSession())
                {
                    var store = new DeviceFlowStore(session, new PersistentGrantSerializer(),
                        FakeLogger<DeviceFlowStore>.Create());
                    await store.RemoveByDeviceCodeAsync(testDeviceCode);
                }

                WaitForIndexing(ravenStore);

                using (var session = ravenStore.OpenSession())
                {
                    session.Query<DeviceFlowCodes>().FirstOrDefault(x => x.UserCode == testUserCode)
                        .Should().BeNull();
                }
            }
        }

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
