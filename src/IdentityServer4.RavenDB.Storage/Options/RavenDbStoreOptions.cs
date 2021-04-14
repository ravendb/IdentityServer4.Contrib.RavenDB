using System;
using Raven.Client.Documents;

namespace IdentityServer4.RavenDB.Storage.Options
{
    public abstract class RavenDbStoreOptions
    {
        public Action<DocumentStore> ConfigureDocumentStore { get; set; }
    }
}