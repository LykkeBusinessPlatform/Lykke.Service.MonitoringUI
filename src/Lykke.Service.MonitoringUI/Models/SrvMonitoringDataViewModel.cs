using System.Collections.Generic;
using Lykke.Service.MonitoringUI.Domain;

namespace Lykke.Service.MonitoringUI.Models
{
    public class SrvMonitoringDataViewModel
    {
        public IEnumerable<MonitoringRecordExtended> MonitoringServiceRecords { get; internal set; }
    }
}
