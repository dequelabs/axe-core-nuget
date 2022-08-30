
namespace Deque.AxeCore.Commons
{
    public class EmbeddedResourceAxeProvider : IAxeScriptProvider
    {
        public string GetScript() => EmbeddedResourceProvider.ReadEmbeddedFile("axe.min.js");
    }
}
