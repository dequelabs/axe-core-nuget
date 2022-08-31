
namespace Deque.AxeCore.Commons
{
    public class BundledAxeScriptProvider : IAxeScriptProvider
    {
        public string GetScript() => BundledAxeScriptProvider.ReadEmbeddedFile("axe.min.js");
    }
}
