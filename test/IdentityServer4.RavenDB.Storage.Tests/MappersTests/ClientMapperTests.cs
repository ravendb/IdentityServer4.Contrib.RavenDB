using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using Xunit;

namespace IdentityServer4.RavenDB.Storage.Tests.MappersTests
{
    public class ClientMapperTests
    {
        [Fact]
        public void ToEntity_MapsAllowedIdentityTokenSigningAlgorithms()
        {
            var client = new Client
            {
                AllowedIdentityTokenSigningAlgorithms = new List<string>
                {
                    "HS256","ES256"
                }
            };

            var entity = client.ToEntity();

            var expectedSigningAlgorithms = "HS256,ES256";
            
            Assert.Equal(expectedSigningAlgorithms, entity.AllowedIdentityTokenSigningAlgorithms);
        }
        
        [Fact]
        public void ToModel_MapsAllowedIdentityTokenSigningAlgorithms()
        {
            var client = new Entities.Client()
            {
                AllowedIdentityTokenSigningAlgorithms = "HS256,ES256"
                
            };

            var entity = client.ToModel();

            Assert.NotNull(entity.AllowedIdentityTokenSigningAlgorithms);
            Assert.NotEmpty(entity.AllowedIdentityTokenSigningAlgorithms);

            var algorithms = entity.AllowedIdentityTokenSigningAlgorithms.ToList();
            var algorithmOne = algorithms[0];
            var algorithmTwo = algorithms[1];
            
            Assert.Equal("HS256", algorithmOne);
            Assert.Equal("ES256", algorithmTwo);
        }
    }
}