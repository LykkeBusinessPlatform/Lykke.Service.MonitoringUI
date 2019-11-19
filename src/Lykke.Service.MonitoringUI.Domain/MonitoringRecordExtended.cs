using System;

namespace Lykke.Service.MonitoringUI.Domain
{
    public class MonitoringRecordExtended
    {
        public DateTime DateTime { get; set; }

        public string ServiceName { get; set; }

        public string Version { get; set; }

        public DateTime? SkipUntil { get; set; }

        public string Url { get; set; }
    }
}
