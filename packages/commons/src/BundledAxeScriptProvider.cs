using System.IO;
using System.Text;

namespace Deque.AxeCore.Commons
{
    public class BundledAxeScriptProvider : IAxeScriptProvider
    {
        public static string ReadEmbeddedFile(string manifestResourceName)
        {
            var assembly = typeof(BundledAxeScriptProvider).Assembly;
            var resourceStream = assembly.GetManifestResourceStream(manifestResourceName);
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public string GetScript() => ReadEmbeddedFile($"Deque.AxeCore.Commons.Resources.axe.min.js");
    }
}
