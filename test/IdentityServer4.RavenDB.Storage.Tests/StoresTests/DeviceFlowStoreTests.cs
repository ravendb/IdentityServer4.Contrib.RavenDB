using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Entities;
using IdentityServer4.RavenDB.Storage.Stores;
using IdentityServer4.Stores.Serialization;
using Raven.Client.Documents;
using Xunit;

namespace IdentityServer4.RavenDB.Storage.Tests.StoresTests
{
    public class DeviceFlowStoreTests : IntegrationTestBase
    {
        private readonly IPersistentGrantSerializer _serializer = new PersistentGrantSerializer();

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenSuccessful_ExpectDeviceCodeAndUserCodeStored()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var deviceCode = Guid.NewGuid().ToString();
            var userCode = Guid.NewGuid().ToString();
            var data = new DeviceCode
            {
                ClientId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            
            await store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);
            
            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundDeviceFlowCodes = await session.Query<DeviceFlowCode>().FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

                foundDeviceFlowCodes.Should().NotBeNull();
                foundDeviceFlowCodes?.DeviceCode.Should().Be(deviceCode);
                foundDeviceFlowCodes?.UserCode.Should().Be(userCode);
            }
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenSuccessful_ExpectDataStored()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var deviceCode = Guid.NewGuid().ToString();
            var userCode = Guid.NewGuid().ToString();
            var data = new DeviceCode
            {
                ClientId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            await store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);
            
            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundDeviceFlowCodes = await session.Query<DeviceFlowCode>().FirstOrDefaultAsync(x => x.DeviceCode == deviceCode);

                foundDeviceFlowCodes.Should().NotBeNull();
                var deserializedData =
                    new PersistentGrantSerializer().Deserialize<DeviceCode>(foundDeviceFlowCodes?.Data);

                deserializedData.CreationTime.Should().BeCloseTo(data.CreationTime);
                deserializedData.ClientId.Should().Be(data.ClientId);
                deserializedData.Lifetime.Should().Be(data.Lifetime);
            }
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenUserCodeAlreadyExists_ExpectException()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var existingUserCode = $"user_{Guid.NewGuid().ToString()}";
            var deviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] {"openid", "api1"},
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim> {new Claim(JwtClaimTypes.Subject, $"sub_{Guid.NewGuid().ToString()}")}))
            };

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new DeviceFlowCode
                {
                    DeviceCode = $"device_{Guid.NewGuid().ToString()}",
                    UserCode = existingUserCode,
                    ClientId = deviceCodeData.ClientId,
                    SubjectId = deviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                    CreationTime = deviceCodeData.CreationTime,
                    Expiration = deviceCodeData.CreationTime.AddSeconds(deviceCodeData.Lifetime),
                    Data = _serializer.Serialize(deviceCodeData)
                });
                
                await session.SaveChangesAsync();
            }
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            
            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());

            await Assert.ThrowsAsync<Exception>(() =>
                store.StoreDeviceAuthorizationAsync($"device_{Guid.NewGuid().ToString()}", existingUserCode,
                    deviceCodeData));
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_WhenDeviceCodeAlreadyExists_ExpectException()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var existingDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var deviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] {"openid", "api1"},
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim> {new Claim(JwtClaimTypes.Subject, $"sub_{Guid.NewGuid().ToString()}")}))
            };

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new DeviceFlowCode
                {
                    DeviceCode = existingDeviceCode,
                    UserCode = $"user_{Guid.NewGuid().ToString()}",
                    ClientId = deviceCodeData.ClientId,
                    SubjectId = deviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                    CreationTime = deviceCodeData.CreationTime,
                    Expiration = deviceCodeData.CreationTime.AddSeconds(deviceCodeData.Lifetime),
                    Data = _serializer.Serialize(deviceCodeData)
                });
                
                await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());

            await Assert.ThrowsAsync<Exception>(() =>
                store.StoreDeviceAuthorizationAsync(existingDeviceCode, $"user_{Guid.NewGuid().ToString()}",
                    deviceCodeData));
            
        }

        [Fact]
        public async Task FindByUserCodeAsync_WhenUserCodeExists_ExpectDataRetrievedCorrectly()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var testUserCode = $"user_{Guid.NewGuid().ToString()}";

            var expectedSubject = $"sub_{Guid.NewGuid().ToString()}";
            var expectedDeviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] {"openid", "api1"},
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {new Claim(JwtClaimTypes.Subject, expectedSubject)}))
            };

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new DeviceFlowCode
                {
                    DeviceCode = testDeviceCode,
                    UserCode = testUserCode,
                    ClientId = expectedDeviceCodeData.ClientId,
                    SubjectId = expectedDeviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                    CreationTime = expectedDeviceCodeData.CreationTime,
                    Expiration = expectedDeviceCodeData.CreationTime.AddSeconds(expectedDeviceCodeData.Lifetime),
                    Data = _serializer.Serialize(expectedDeviceCodeData)
                });
                
                await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByUserCodeAsync(testUserCode);
            

            code.Should().BeEquivalentTo(expectedDeviceCodeData,
                assertionOptions => assertionOptions.Excluding(x => x.Subject));

            code.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task FindByUserCodeAsync_WhenUserCodeDoesNotExist_ExpectNull()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByUserCodeAsync($"user_{Guid.NewGuid().ToString()}");
            code.Should().BeNull();
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_WhenDeviceCodeExists_ExpectDataRetrievedCorrectly()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var testUserCode = $"user_{Guid.NewGuid().ToString()}";

            var expectedSubject = $"sub_{Guid.NewGuid().ToString()}";
            var expectedDeviceCodeData = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] {"openid", "api1"},
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {new Claim(JwtClaimTypes.Subject, expectedSubject)}))
            };

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new DeviceFlowCode
                {
                    DeviceCode = testDeviceCode,
                    UserCode = testUserCode,
                    ClientId = expectedDeviceCodeData.ClientId,
                    SubjectId = expectedDeviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                    CreationTime = expectedDeviceCodeData.CreationTime,
                    Expiration = expectedDeviceCodeData.CreationTime.AddSeconds(expectedDeviceCodeData.Lifetime),
                    Data = _serializer.Serialize(expectedDeviceCodeData)
                });
                
                await session.SaveChangesAsync();
            }
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByDeviceCodeAsync(testDeviceCode);
            

            code.Should().BeEquivalentTo(expectedDeviceCodeData,
                assertionOptions => assertionOptions.Excluding(x => x.Subject));

            code.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_WhenDeviceCodeDoesNotExist_ExpectNull()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            var code = await store.FindByDeviceCodeAsync($"device_{Guid.NewGuid().ToString()}");
            code.Should().BeNull();
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_WhenDeviceCodeAuthorized_ExpectSubjectAndDataUpdated()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var testUserCode = $"user_{Guid.NewGuid().ToString()}";

            var expectedSubject = $"sub_{Guid.NewGuid().ToString()}";
            var unauthorizedDeviceCode = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] {"openid", "api1"},
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300,
                IsOpenId = true
            };

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new DeviceFlowCode
                {
                    DeviceCode = testDeviceCode,
                    UserCode = testUserCode,
                    ClientId = unauthorizedDeviceCode.ClientId,
                    CreationTime = unauthorizedDeviceCode.CreationTime,
                    Expiration = unauthorizedDeviceCode.CreationTime.AddSeconds(unauthorizedDeviceCode.Lifetime),
                    Data = _serializer.Serialize(unauthorizedDeviceCode)
                });
                
                await session.SaveChangesAsync();
            }
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var authorizedDeviceCode = new DeviceCode
            {
                ClientId = unauthorizedDeviceCode.ClientId,
                RequestedScopes = unauthorizedDeviceCode.RequestedScopes,
                AuthorizedScopes = unauthorizedDeviceCode.RequestedScopes,
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {new Claim(JwtClaimTypes.Subject, expectedSubject)})),
                IsAuthorized = true,
                IsOpenId = true,
                CreationTime = new DateTime(2018, 10, 19, 16, 14, 29),
                Lifetime = 300
            };

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            await store.UpdateByUserCodeAsync(testUserCode, authorizedDeviceCode);
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            using (var session = storeHolder.OpenAsyncSession())
            {
                var updatedCodes = await session.Query<DeviceFlowCode>().SingleAsync(x => x.UserCode == testUserCode);
                
                // should be unchanged
                updatedCodes.DeviceCode.Should().Be(testDeviceCode);
                updatedCodes.ClientId.Should().Be(unauthorizedDeviceCode.ClientId);
                updatedCodes.CreationTime.Should().Be(unauthorizedDeviceCode.CreationTime);
                updatedCodes.Expiration.Should()
                    .Be(unauthorizedDeviceCode.CreationTime.AddSeconds(authorizedDeviceCode.Lifetime));

                // should be changed
                var parsedCode = _serializer.Deserialize<DeviceCode>(updatedCodes.Data);
                parsedCode.Should().BeEquivalentTo(authorizedDeviceCode,
                    assertionOptions => assertionOptions.Excluding(x => x.Subject));
                parsedCode.Subject.Claims
                    .FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject).Should()
                    .NotBeNull();
            }
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_WhenDeviceCodeExists_ExpectDeviceCodeDeleted() 
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

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

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(new DeviceFlowCode
                {
                    DeviceCode = testDeviceCode,
                    UserCode = testUserCode,
                    ClientId = existingDeviceCode.ClientId,
                    CreationTime = existingDeviceCode.CreationTime,
                    Expiration = existingDeviceCode.CreationTime.AddSeconds(existingDeviceCode.Lifetime),
                    Data = _serializer.Serialize(existingDeviceCode)
                });
                
                await session.SaveChangesAsync();
            }
                
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(),
                FakeLogger<DeviceFlowStore>.Create());
            await store.RemoveByDeviceCodeAsync(testDeviceCode);
            
            using (var session = storeHolder.OpenAsyncSession())
            {
                var deviceFlowCode = await session.Query<DeviceFlowCode>()
                    .FirstOrDefaultAsync(x => x.UserCode == testUserCode);
                
                deviceFlowCode.Should().BeNull();
            }
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_WhenDeviceCodeDoesNotExists_ExpectSuccess()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();
            
            var store = new DeviceFlowStore(storeHolder, new PersistentGrantSerializer(), FakeLogger<DeviceFlowStore>.Create());

            await store.RemoveByDeviceCodeAsync($"device_{Guid.NewGuid().ToString()}");
        }
    }
}
