﻿namespace Deque.AxeCore.Commons
{
    public class AxeResultCheck
    {
        public object Data { get; set; }
        public string Id { get; set; }
        public string Impact { get; set; }
        public string Message { get; set; }
        public AxeResultRelatedNode[] RelatedNodes { get; set; }
    }
}
