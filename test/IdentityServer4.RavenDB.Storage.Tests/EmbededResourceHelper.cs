using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IdentityServer4.RavenDB.Storage.Tests
{
    public static class EmbeddedResourceHelper
    {
        public static string GetFileContent(string resourcePath)
        {
            using (var stream = GetStream(resourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static Stream GetStream(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourcePath);
            
            if (stream == null)
                throw new FileNotFoundException($"Could not find embedded resource {resourcePath}", resourcePath);

            return stream;
        }

        public static IEnumerable<string> GetContentLines(string resourcePath)
        {
            using (var stream = GetStream(resourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                var line = streamReader.ReadLine();
                
                while (line != null)
                {
                    yield return line;
                    line = streamReader.ReadLine();
                }
            }
        }
    }
}