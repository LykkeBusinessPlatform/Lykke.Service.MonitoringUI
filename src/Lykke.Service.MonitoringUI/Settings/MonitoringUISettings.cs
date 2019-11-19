using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.MonitoringUI.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class MonitoringUISettings
    {
        public DbSettings Db { get; set; }
    }
}
