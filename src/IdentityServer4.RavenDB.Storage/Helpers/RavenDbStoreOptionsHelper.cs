using System;
using IdentityServer4.RavenDB.Storage.Options;

namespace IdentityServer4.RavenDB.Storage.Helpers
{
    internal static class RavenDbStoreOptionsHelper
    {
        public static T GetRavenDbStoreOptions<T>(Action<T> ravenDbStoreOptionsAction) where T : RavenDbStoreOptions, new()
        {
            var options = new T();
            ravenDbStoreOptionsAction(options);
            return options;
        }
    }
}