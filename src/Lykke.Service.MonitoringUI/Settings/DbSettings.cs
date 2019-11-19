﻿using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.MonitoringUI.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
