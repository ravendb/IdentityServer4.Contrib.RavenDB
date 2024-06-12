using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.RavenDB.Storage.Mappers;
using NLog;
using NLog.Config;
using NLog.Targets;
using Raven.Client.Documents;

namespace IdentityServer4.RavenDB.Storage.Migration
{
    public static class IdentityServerDataMigrator
    {
        private static readonly Logger Logger;
        
        static IdentityServerDataMigrator()
        {
            Logger = ConfigureLogger();
        }
        
        public static async Task Run(ConfigurationData data, DocumentStore documentStore)
        {
            if (documentStore == null)
            {
                Logger.Info($"Document store has not been provided. Configure the {nameof(DocumentStore)} object and try again. Exiting...");
                return;
            }
            
            if (data == null)
            {
                Logger.Info("Found no data to migrate. Exiting...");
                return;
            }
            
            var apiResources = data.ApiResources?.ToList();
            var apiScopes = data.ApiScopes?.ToList();
            var clients = data.Clients?.ToList();
            var identityResources = data.IdentityResources?.ToList();

            var configDataNotProvided = (apiResources == null || apiResources.Any() == false) &&
                                        (apiScopes == null || apiScopes.Any() == false) &&
                                        (clients == null || clients.Any() == false) &&
                                        (identityResources == null || identityResources.Any() == false);

            if (configDataNotProvided)
            {
                Logger.Info("Found no data to migrate. Exiting...");
                return;
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            using (var session = documentStore.OpenAsyncSession())
            {
                Logger.Info($"Starting data migration to {documentStore.Database} database...");

                if (apiResources != null && apiResources.Any())
                {
                    Logger.Info($"Migrating {nameof(data.ApiResources)}...");
                    
                    foreach (var apiResource in apiResources)
                    {
                        var entity = apiResource.ToEntity();
                        await session.StoreAsync(entity);
                    }
                }

                if (apiScopes != null && data.ApiScopes.Any())
                {
                    Logger.Info($"Migrating {nameof(data.ApiScopes)}...");
                    
                    foreach (var apiScope in apiScopes)
                    {
                        var entity = apiScope.ToEntity();
                        await session.StoreAsync(entity);
                    }
                }

                if (clients != null && data.Clients.Any())
                {
                    Logger.Info($"Migrating {nameof(data.Clients)}...");
                    
                    foreach (var client in clients)
                    {
                        var entity = client.ToEntity();
                        await session.StoreAsync(entity);
                    }
                }

                if (identityResources != null && data.IdentityResources.Any())
                {
                    Logger.Info($"Migrating {nameof(data.IdentityResources)}...");
                    
                    foreach (var identityResource in identityResources)
                    {
                        var entity = identityResource.ToEntity();
                        await session.StoreAsync(entity);
                    }
                }

                await session.SaveChangesAsync();

                stopWatch.Stop();
                var timespan = stopWatch.Elapsed;

                Logger.Info(
                    $"Migration completed successfully. Elapsed time: {timespan:hh\\:mm\\:ss\\.ffff} ." );
            }
        }

        private static Logger ConfigureLogger()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = "${date:format=HH\\:mm\\:ss} ${logger} ${message}";
            var rule = new LoggingRule("*", LogLevel.Info, consoleTarget);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
            return LogManager.GetCurrentClassLogger();
        }
    }

    public class ConfigurationData
    {
        public IEnumerable<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public IEnumerable<ApiScope> ApiScopes { get; set; } = new List<ApiScope>();
        public IEnumerable<Client> Clients { get; set; } = new List<Client>();
        public IEnumerable<IdentityResource> IdentityResources { get; set; } = new List<IdentityResource>();
    }
}