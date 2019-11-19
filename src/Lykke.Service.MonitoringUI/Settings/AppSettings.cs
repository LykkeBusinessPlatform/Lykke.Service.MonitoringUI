using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.MonitoringUI.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public MonitoringUISettings MonitoringUIService { get; set; }
    }
}
