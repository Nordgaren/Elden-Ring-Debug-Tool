using Bluegrams.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class SettingsViewViewModel : ViewModelBase
    {
        internal static Properties.Settings Settings { get; }

        static SettingsViewViewModel()
        {
            PortableJsonSettingsProvider.SettingsFileName = "ERDebug.settings.json";
            PortableSettingsProviderBase.SettingsDirectory = Environment.CurrentDirectory;
            PortableJsonSettingsProvider.ApplyProvider(Properties.Settings.Default);
            Settings = Properties.Settings.Default;
        }

        public void Dispose()
        {
            Settings.Save();
        }

        private bool _showWarning = Settings.ShowWarning;
        public bool ShowWarning
        {
            get => _showWarning;
            set
            {
                if (SetField(ref _showWarning, value))
                {
                    Settings.ShowWarning = ShowWarning;
                }
            }
        }

        private bool _enableHotKeys = Settings.EnableHotkeys;
        public bool EnableHotKeys
        {
            get => _enableHotKeys;
            set
            {
                if (SetField(ref _enableHotKeys, value))
                {
                    Settings.EnableHotkeys = EnableHotKeys;
                }
            }
        }

        private bool _spawnUntradable = Settings.SpawnUntradeable;
        public bool SpawnUntradeable
        {
            get => _spawnUntradable;
            set
            {
                if (SetField(ref _spawnUntradable, value))
                {
                    Settings.SpawnUntradeable = SpawnUntradeable;
                }
            }
        }

    }
}
