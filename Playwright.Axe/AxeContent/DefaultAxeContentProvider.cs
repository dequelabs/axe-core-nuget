#nullable enable

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Playwright.Axe.AxeContent
{
    /// <inheritdoc />
    public sealed class DefaultAxeContentProvider : IAxeContentProvider
    {
        /// <inheritdoc />
        public string GetAxeCoreScriptContent()
        {
            var resourceStream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("Playwright.Axe.axe.js");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
