using System;
using System.Runtime.CompilerServices;
using Raven.Client.Documents;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.DocumentStoreHolder
{
    internal class ConfigurationDocumentStoreHolder : DocumentStoreHolderBase, IDisposable
    {
        public ConfigurationDocumentStoreHolder(IDocumentStore documentStore) : base(documentStore)
        {
        }
    }
}