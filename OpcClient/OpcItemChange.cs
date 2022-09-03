using System;

namespace OpcClient
{
    public class OpcItemChange
    {
        public DateTime SnapTime { get; set; }
        public string Address { get; set; }
        public string Descriptor { get; set; }
        public string ValueOld { get; set; }
        public string ValueNew { get; set; }
    }
}