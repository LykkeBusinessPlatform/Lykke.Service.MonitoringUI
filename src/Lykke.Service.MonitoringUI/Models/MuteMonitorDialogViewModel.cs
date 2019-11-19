namespace Lykke.Service.MonitoringUI.Models
{
    public class MuteMonitorDialogViewModel : IPersonalAreaDialog
    {
        public string ServiceName { get; set; }

        public int Minutes { get; set; }

        public string Caption { get; set; }

        public string Width { get; set; }
    }
}
