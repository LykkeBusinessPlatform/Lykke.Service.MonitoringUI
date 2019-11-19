using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.MonitoringUI.Domain.Services
{
    public interface IMonitoringServiceCallerService
    {
        Task<IEnumerable<MonitoringRecordExtended>> GetAllAsync();
        Task MuteServiceAsync(string serviceName, int minutes);
        Task UnMuteServiceAsync(string serviceName);
        Task RemoveUrlFromMonitoring(string serviceName);
        Task AddUrlToMonitoringAsync(string serviceName, string url);
        Task<MonitoringRecordExtended> GetServiceAsync(string serviceName);
    }
}
