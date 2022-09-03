using System;

namespace OpcClient
{
    public class OpcItemAnswer
    {
        public OpcItemAnswer(string address, string content, string descriptor)
        {
            Address = address;
            Descriptor = descriptor;
            var values = content.Split(';');
            if (values.Length != 3) return;
            if (ushort.TryParse(values[0], out var value))
                Value = value;
            Quality = values[1];
            if (DateTime.TryParse(values[2], System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out var date))
                SnapTime = date.ToLocalTime();
        }

        public string Address { get; set; }
        public ushort Value { get; set; }
        public string Descriptor { get; set; }
        public string Quality { get; set; }
        public DateTime SnapTime { get; set; }

        public string GetItem()
        {
            var values = Address.Split('.');
            return values.Length == 6 ? values[5] : values[0];
        }
    }
}