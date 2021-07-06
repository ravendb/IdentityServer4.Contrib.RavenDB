using System;
using Raven.Client.Documents.Session;

namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    public interface IDocumentStoreHolder : IDisposable
    {
        IAsyncDocumentSession OpenAsyncSession();
    }
}