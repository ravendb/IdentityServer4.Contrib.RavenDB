using IdentityModel;
using IdentityServer4.RavenDB.Storage.Helpers;
using IdentityServer4.RavenDB.Storage.Mappers;
using Xunit;

namespace IdentityServer4.RavenDB.IntegrationTests.Mappers
{
    public class PersistedGrantMapperTests
    {
        [Fact]
        public void ToEntity_MapsKeyWithDefaultHash()
        {
            const string key = "some-key";
            
            var persistedGrant = new Models.PersistedGrant()
            {
                Key = key
            };

            var entity = persistedGrant.ToEntity();

            var expectedKeyValue = CryptographyHelper.CreateHash(key);
            
            Assert.Equal(expectedKeyValue, entity.Key);
        }

        [Fact]
        public void ToModel_DoesNotChangeKeyHashValue()
        {
            const string key = "some-key";

            var hashedKeyValue = CryptographyHelper.CreateHash(key);
            
            var entity = new Storage.Entities.PersistedGrant
            {
                Key = hashedKeyValue
            };

            var model = entity.ToModel();
            
            Assert.Equal(hashedKeyValue, model.Key);
        }
    }
}