using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.MonitoringServiceApiCaller;
using Lykke.Service.MonitoringUI.Domain;
using Lykke.Service.MonitoringUI.Domain.Services;

namespace Lykke.Service.MonitoringUI.DomainServices
{
    public class MonitoringServiceCallerService : IMonitoringServiceCallerService
    {
        private readonly MonitoringServiceFacade _monitoringServiceFacade;


        [UsedImplicitly]
        public MonitoringServiceCallerService(MonitoringServiceFacade monitoringServiceFacade)
        {
            _monitoringServiceFacade = monitoringServiceFacade;
        }

        public async Task<IEnumerable<MonitoringRecordExtended>> GetAllAsync()
        {
            var allServices = await _monitoringServiceFacade.GetAll();
            var orderedByName = allServices.OrderBy(x => x.ServiceName).ToList();
            var records = new List<MonitoringRecordExtended>(orderedByName.Count);

            foreach (var service in orderedByName)
            {
                records.Add(new MonitoringRecordExtended
                {
                    DateTime = service.LastPing ?? DateTime.MinValue,
                    ServiceName = service.ServiceName,
                    Version = service.Version,
                    SkipUntil = service.SkipUntil,
                    Url = service.Url
                });
            }

            return records;
        }

        public async Task<MonitoringRecordExtended> GetServiceAsync(string serviceName)
        {
            var record = await _monitoringServiceFacade.GetService(serviceName);
            return new MonitoringRecordExtended
            {
                DateTime = record.LastPing ?? DateTime.MinValue,
                ServiceName = record.ServiceName,
                Version = record.Version,
                Url = record.Url,
                SkipUntil = record.SkipUntil,
            };
        }

        public async Task MuteServiceAsync(string serviceName, int minutes)
        {
            await _monitoringServiceFacade.Mute(new MonitoringServiceApiCaller.Models.MonitoringObjectMuteModel()
            {
                Minutes = minutes,
                ServiceName = serviceName
            });
        }

        public async Task UnMuteServiceAsync(string serviceName)
        {
            await _monitoringServiceFacade.Unmute(new MonitoringServiceApiCaller.Models.MonitoringObjectUnmuteModel()
            {
                ServiceName = serviceName
            });
        }

        public async Task RemoveUrlFromMonitoring(string serviceName)
        {
            await _monitoringServiceFacade.RemoveService(serviceName);
        }

        public async Task AddUrlToMonitoringAsync(string serviceName, string url)
        {
            await _monitoringServiceFacade.MonitorUrl(new MonitoringServiceApiCaller.Models.UrlMonitoringObjectModel()
            {
                ServiceName = serviceName,
                Url = url
            });
        }
    }
}
