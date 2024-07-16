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
        
        private bool _init = Settings.Init;
        public bool Init
        {
            get => _init;
            set
            {
                if (SetField(ref _init, value))
                {
                    Settings.Init = Init;
                }
            }
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

        private bool _enableHotKeys = Settings.EnableHotKeys;
        public bool EnableHotKeys
        {
            get => _enableHotKeys;
            set
            {
                if (SetField(ref _enableHotKeys, value))
                {
                    Settings.EnableHotKeys = EnableHotKeys;
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

        private bool _editInventoryCounts = Settings.EditInventoryCounts;
        public bool EditInventoryCounts
        {
            get => _editInventoryCounts;
            set
            {
                if (SetField(ref _editInventoryCounts, value))
                {
                    Settings.EditInventoryCounts = EditInventoryCounts;
                }
            }
        }

        private TimeSpan _loadTimeout = Settings.LoadTimeout;
        public TimeSpan LoadTimeout
        {
            get => _loadTimeout;
            set
            {
                if (SetField(ref _loadTimeout, value))
                {
                    Settings.LoadTimeout = LoadTimeout;
                }
            }
        }
        
        private bool _hideDlc = Settings.HideDlc;
        public bool HideDlc
        {
            get => _hideDlc;
            set
            {
                if (SetField(ref _hideDlc, value))
                {
                    Settings.HideDlc = HideDlc;
                }
            }
        }
    }
}
