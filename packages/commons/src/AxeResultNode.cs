namespace Deque.AxeCore.Commons
{
    public class AxeResultNode
    {
        public AxeSelector Target { get; set; }
        public AxeSelector XPath { get; set; }
        public string Html { get; set; }
        public string Impact { get; set; }
        public AxeResultCheck[] Any { get; set; }
        public AxeResultCheck[] All { get; set; }
        public AxeResultCheck[] None { get; set; }
    }
}
