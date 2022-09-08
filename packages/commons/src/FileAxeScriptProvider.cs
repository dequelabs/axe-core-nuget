using System;
using System.IO;

namespace Deque.AxeCore.Commons
{
    public class FileAxeScriptProvider : IAxeScriptProvider
    {
        private readonly string _filePath;

        public FileAxeScriptProvider(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exists", nameof(filePath));

            _filePath = filePath;
        }

        public string GetScript()
        {
            if (!File.Exists(_filePath))
                throw new InvalidOperationException($"File '{_filePath}' does not exist");

            return File.ReadAllText(_filePath);
        }
    }
}
