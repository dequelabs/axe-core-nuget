using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright
{
    internal static class EmbeddedResourceProvider
    {
        public static Task<string> ReadEmbeddedFileAsync(string fileName)
        {
            var assembly = typeof(EmbeddedResourceProvider).Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"Deque.AxeCore.Playwright.Resources.{fileName}");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEndAsync();
            }
        }
    }
}
