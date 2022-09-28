using System.IO;
using System.Text;

namespace Deque.AxeCore.Playwright
{
    internal static class EmbeddedResourceProvider
    {
        public static string ReadEmbeddedFile(string fileName)
        {
            var assembly = typeof(EmbeddedResourceProvider).Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"Deque.AxeCore.Playwright.Resources.{fileName}");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
