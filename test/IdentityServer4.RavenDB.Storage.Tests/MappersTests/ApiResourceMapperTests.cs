using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using Xunit;

namespace IdentityServer4.RavenDB.Storage.Tests.MappersTests
{
    public class ApiResourceMapperTests
    {
        [Fact]
        public void ToEntity_MapsAllowedAccessTokenSigningAlgorithms()
        {
            var apiResource = new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = new List<string>
                {
                    "HS256","ES256"
                }
            };

            var entity = apiResource.ToEntity();

            var expectedSigningAlgorithms = "HS256,ES256";
            
            Assert.Equal(expectedSigningAlgorithms, entity.AllowedAccessTokenSigningAlgorithms);
        }
        
        [Fact]
        public void ToModel_MapsAllowedIdentityTokenSigningAlgorithms()
        {
            var apiResource = new Entities.ApiResource()
            {
                AllowedAccessTokenSigningAlgorithms = "HS256,ES256"
            };

            var entity = apiResource.ToModel();

            Assert.NotNull(apiResource.AllowedAccessTokenSigningAlgorithms);
            Assert.NotEmpty(entity.AllowedAccessTokenSigningAlgorithms);
            
            var algorithms = entity.AllowedAccessTokenSigningAlgorithms.ToList();
            var algorithmOne = algorithms[0];
            var algorithmTwo = algorithms[1];
            
            Assert.Equal("HS256", algorithmOne);
            Assert.Equal("ES256", algorithmTwo);
        }
    }
}