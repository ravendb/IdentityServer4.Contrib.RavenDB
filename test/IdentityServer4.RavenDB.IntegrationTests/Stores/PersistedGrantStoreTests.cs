using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Stores;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Stores
{
    public class PersistedGrantStoreTests : IntegrationTest
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
        public async Task Store_should_update_record_if_key_already_exists()
        {
            using (var ravenStore = GetDocumentStore())
            {
                var persistedGrant = CreateTestObject();

                using (var session = ravenStore.OpenSession())
                {
                    session.Store(persistedGrant.ToEntity());
                    session.SaveChanges();
                }

                var newDate = persistedGrant.Expiration.Value.AddHours(1);
                using (var session = ravenStore.OpenAsyncSession())
                {
                    var store = new PersistedGrantStore(session, FakeLogger<PersistedGrantStore>.Create());
                    persistedGrant.Expiration = newDate;
                    await store.StoreAsync(persistedGrant);
                }

                using (var session = ravenStore.OpenSession())
                {
                    var foundGrant = session.Query<Storage.Entities.PersistedGrant>()
                        .FirstOrDefault(x => x.Key == persistedGrant.Key);
                    Assert.NotNull(foundGrant);
                    Assert.Equal(newDate, persistedGrant.Expiration);
                }
            }
        }
    }
}
