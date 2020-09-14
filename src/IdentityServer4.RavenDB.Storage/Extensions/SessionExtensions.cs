using System.Threading.Tasks;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

namespace IdentityServer4.Contrib.RavenDB.Extensions
{
    internal static class SessionExtensions
    {
        public static Task WaitForIndexAndSaveChangesAsync<T>(this IAsyncDocumentSession session) where T : AbstractIndexCreationTask
        {
            session.Advanced.WaitForIndexesAfterSaveChanges(indexes: new [] { typeof(T).Name });
            return session.SaveChangesAsync();

        }
    }
}