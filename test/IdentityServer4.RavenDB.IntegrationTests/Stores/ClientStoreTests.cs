using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using IdentityServer4.RavenDB.Storage.Stores;
using Xunit;
using Xunit.Sdk;

namespace IdentityServer4.RavenDB.IntegrationTests.Stores
{
    public class ClientStoreTests : IntegrationTest
    {
        [Fact]
        public async Task FindClientByIdAsync_WhenClientsExistWithManyCollections_ExpectClientReturnedInUnderFiveSeconds()
        {
            using (var ravenStore = GetDocumentStore())
            {
                var testClient = new Client
                {
                    ClientId = "test_client_with_uris",
                    ClientName = "Test client with URIs",
                    AllowedScopes = {"openid", "profile", "api1"},
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials
                };

                for (int i = 0; i < 50; i++)
                {
                    testClient.RedirectUris.Add($"https://localhost/{i}");
                    testClient.PostLogoutRedirectUris.Add($"https://localhost/{i}");
                    testClient.AllowedCorsOrigins.Add($"https://localhost:{i}");
                }

                using (var session = ravenStore.OpenSession())
                {
                    session.Store(testClient.ToEntity());

                    for (int i = 0; i < 50; i++)
                    {
                        session.Store(new Client
                        {
                            ClientId = testClient.ClientId + i,
                            ClientName = testClient.ClientName,
                            AllowedScopes = testClient.AllowedScopes,
                            AllowedGrantTypes = testClient.AllowedGrantTypes,
                            RedirectUris = testClient.RedirectUris,
                            PostLogoutRedirectUris = testClient.PostLogoutRedirectUris,
                            AllowedCorsOrigins = testClient.AllowedCorsOrigins,
                        }.ToEntity());
                    }

                    session.SaveChanges();
                }

                using (var session = ravenStore.OpenAsyncSession())
                {
                    var store = new ClientStore(session, FakeLogger<ClientStore>.Create());

                    const int timeout = 5000;
                    var task = Task.Run(() => store.FindClientByIdAsync(testClient.ClientId));

                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        var client = task.Result;
                        client.Should().BeEquivalentTo(testClient);
                    }
                    else
                    {
                        throw new TestTimeoutException(timeout);
                    }
                }
            }
        }
    }
}
