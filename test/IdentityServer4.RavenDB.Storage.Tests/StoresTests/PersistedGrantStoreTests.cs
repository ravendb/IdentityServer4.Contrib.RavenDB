using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Indexes;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Stores;
using IdentityServer4.Stores;
using Raven.Client.Documents;
using Xunit;

namespace IdentityServer4.RavenDB.Storage.Tests.StoresTests
{
    public class PersistedGrantStoreTests : IntegrationTestBase
    {
        private static PersistedGrant CreateTestObject()
        {
            return new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = "authorization_code",
                ClientId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();

            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());
            await store.StoreAsync(persistedGrant);
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var hashedTokenKey = CryptographyHelper.CreateHash(persistedGrant.Key);

            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundGrant = await session.Query<PersistedGrant>().FirstOrDefaultAsync(x => x.Key == hashedTokenKey);
                Assert.NotNull(foundGrant);
            }
        }

        [Fact]
        public async Task GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();
            var entity = persistedGrant.ToEntity();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(entity);
               await session.SaveChangesAsync();
            }
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());

            var foundPersistedGrant = await store.GetAsync(entity.Key);

            Assert.NotNull(foundPersistedGrant);
        }

        [Fact]
        public async Task GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(persistedGrant.ToEntity());
                await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());
            var filter = new PersistedGrantFilter
            {
                SubjectId = persistedGrant.SubjectId
            };
            
            var foundPersistedGrants = (await store.GetAllAsync(filter)).ToList();

            Assert.NotNull(foundPersistedGrants);
            Assert.NotEmpty(foundPersistedGrants);
        }

        [Fact]
        public async Task RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(persistedGrant.ToEntity());
                await session.SaveChangesAsync();
            }

            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAsync(persistedGrant.Key);
            
            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundGrant = await session.Query<PersistedGrant>()
                    .FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Fact]
        public async Task RemoveAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(persistedGrant.ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());
            var filter = new PersistedGrantFilter
            {
                SubjectId = persistedGrant.SubjectId,
                ClientId = persistedGrant.ClientId
            };
            
            await store.RemoveAllAsync(filter);
            
            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundGrant = await session.Query<PersistedGrant, PersistedGrantIndex>()
                    .FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Fact]
        public async Task RemoveAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();

            using (var session = storeHolder.OpenAsyncSession())
            {
               await session.StoreAsync(persistedGrant.ToEntity());
               await session.SaveChangesAsync();
            }

            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());

            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());
            var filter = new PersistedGrantFilter
            {
                SubjectId = persistedGrant.SubjectId,
                ClientId = persistedGrant.ClientId,
                Type = persistedGrant.Type
            };
            
            await store.RemoveAllAsync(filter);

            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundGrant = await session.Query<PersistedGrant>()
                    .FirstOrDefaultAsync(x => x.Key == persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Fact]
        public async Task Store_should_create_new_record_if_key_does_not_exist()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();

            var hashedTokenKey = CryptographyHelper.CreateHash(persistedGrant.Key);

            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundGrant = await session.Query<PersistedGrant>().FirstOrDefaultAsync(x => x.Key == hashedTokenKey);
                Assert.Null(foundGrant);
            }

            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());
            await store.StoreAsync(persistedGrant);
            
            WaitForIndexing(storeHolder.IntegrationTest_GetDocumentStore());
            WaitForUserToContinueTheTest(storeHolder.IntegrationTest_GetDocumentStore());

            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundGrant = await session.Query<PersistedGrant>()
                    .FirstOrDefaultAsync(x => x.Key == hashedTokenKey);
                Assert.NotNull(foundGrant);
            }
        }

        [Fact]
        public async Task Store_should_update_record_if_key_already_exists()
        {
            var storeHolder = GetOperationalDocumentStoreHolder();

            var persistedGrant = CreateTestObject();
            var entity = persistedGrant.ToEntity();

            using (var session = storeHolder.OpenAsyncSession())
            {
                await session.StoreAsync(entity);
                await session.SaveChangesAsync();
            }

            var newDate = persistedGrant.Expiration.Value.AddHours(1);
            
            var store = new PersistedGrantStore(storeHolder, FakeLogger<PersistedGrantStore>.Create());
            persistedGrant.Expiration = newDate;
            await store.StoreAsync(persistedGrant);

            var hashedTokenKey = CryptographyHelper.CreateHash(persistedGrant.Key);
            
            using (var session = storeHolder.OpenAsyncSession())
            {
                var foundGrant = await session.Query<Storage.Entities.PersistedGrant>()
                    .FirstOrDefaultAsync(x => x.Key == hashedTokenKey);
                Assert.NotNull(foundGrant);
                Assert.Equal(newDate, persistedGrant.Expiration);
            }
        }
        
        //[Fact]
        //public async Task GetAllAsync_is_implemented()
        //{
        //    using (var ravenStore = GetDocumentStore())
        //    {
        //        var persistedGrant = CreateTestObject();

        //        using (var session = ravenStore.OpenSession())
        //        {
        //            session.Store(persistedGrant.ToEntity());
        //            session.SaveChanges();
        //        }

        //        var newDate = persistedGrant.Expiration.Value.AddHours(1);
        //        using (var session = ravenStore.OpenAsyncSession())
        //        {
        //            var store = new PersistedGrantStore(session, FakeLogger<PersistedGrantStore>.Create());

        //            PersistedGrantFilter filter = new PersistedGrantFilter();
        //            await store.GetAllAsync(filter);
        //        }
        //    }
        //}

        //[Fact]
        //public async Task RemoveAllAsync_is_implemented()
        //{
        //    using (var ravenStore = GetDocumentStore())
        //    {
        //        var persistedGrant = CreateTestObject();

        //        using (var session = ravenStore.OpenSession())
        //        {
        //            session.Store(persistedGrant.ToEntity());
        //            session.SaveChanges();
        //        }

        //        var newDate = persistedGrant.Expiration.Value.AddHours(1);
        //        using (var session = ravenStore.OpenAsyncSession())
        //        {
        //            var store = new PersistedGrantStore(session, FakeLogger<PersistedGrantStore>.Create());

        //            PersistedGrantFilter filter = new PersistedGrantFilter();
        //            await store.RemoveAllAsync(filter);
        //        }
        //    }
        //}
    }
}
