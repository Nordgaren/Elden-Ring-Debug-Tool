using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Erd_Tools;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class MiscViewViewModel : ViewModelBase
    {
        public ErdHook Hook { get; private set; }
        public ICommand EnableEventFlag { get; }
        public ICommand DisableEventFlag { get; }

        public MiscViewViewModel()
        {
            EnableEventFlag = new EnableEventCommand(this);
            DisableEventFlag = new DisableEventCommand(this);
        }

        public void UpdateViewModel()
        {
            Loaded = Hook.Loaded;
            Setup = Hook.Setup;
        }

        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
        }

        private bool _setup;
        public bool Setup
        {
            get => _setup;
            set => SetField(ref _setup, value);
        }

        private bool _loaded;
        public bool Loaded
        {
            get => _loaded;
            set => SetField(ref _loaded, value);
        }

        private int _eventFlag;

        public int EventFlag
        {
            get => _eventFlag;
            set => SetField(ref _eventFlag, value);
        }
    }
}
