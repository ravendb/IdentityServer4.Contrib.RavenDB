using System;
using IdentityServer4.RavenDB.Storage.Options;

namespace IdentityServer4.RavenDB.Storage.Helpers
{
    internal static class RavenDbStoreOptionsHelper
    {
        public static T GetOptions<T>(Action<T> configureStoreOptions) where T : RavenDbStoreOptions, new()
        {
            var options = new T();
            configureStoreOptions(options);
            return options;
        }
    }
}