
namespace Deque.AxeCore.Commons
{
    internal class EmbeddedResourceAxeProvider : IAxeScriptProvider
    {
        public string GetScript() => EmbeddedResourceProvider.ReadEmbeddedFile("axe.min.js");
    }
}
