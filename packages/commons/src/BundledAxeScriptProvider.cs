
namespace Deque.AxeCore.Commons
{
    public class BundledAxeScriptProvider : IAxeScriptProvider
    {
        public string GetScript() => EmbeddedResourceProvider.ReadEmbeddedFile("axe.min.js");
    }
}
