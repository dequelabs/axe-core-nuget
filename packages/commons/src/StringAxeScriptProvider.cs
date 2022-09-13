using System;

namespace Deque.AxeCore.Commons
{
    public class StringAxeScriptProvider : IAxeScriptProvider
    {
        private readonly string _axeSource;

        public StringAxeScriptProvider(string axeSource)
        {
            _axeSource = axeSource;
        }

        public string GetScript()
        {
            return _axeSource;
        }
    }
}

