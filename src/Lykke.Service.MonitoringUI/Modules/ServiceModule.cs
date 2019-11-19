using Autofac;
using JetBrains.Annotations;
using Lykke.MonitoringServiceApiCaller;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Service.MonitoringUI.Domain.Services;
using Lykke.Service.MonitoringUI.DomainServices;
using Lykke.Service.MonitoringUI.Services;
using Lykke.Service.MonitoringUI.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.MonitoringUI.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly AppSettings _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<MonitoringServiceCallerService>()
                .As<IMonitoringServiceCallerService>()
                .SingleInstance();

            builder.RegisterInstance(new MonitoringServiceFacade(_appSettings.MonitoringServiceClient.MonitoringServiceUrl));
        }
    }
}
