using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ICommand CheckEventFlag { get; }

        public MiscViewViewModel()
        {
            EnableEventFlag = new EnableEventCommand(this);
            DisableEventFlag = new DisableEventCommand(this);
            CheckEventFlag = new IsEventCommand(this);
            EventFlags = new ObservableCollection<EventFlagItem>();
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
            set
            {
                if (SetField(ref _eventFlag, value))
                {
                    IsEventFlag = null;
                }
            }
        }

        private bool? _isEventFlag;
        public bool? IsEventFlag
        {
            get => _isEventFlag;
            set => SetField(ref _isEventFlag, value);
        }

        // Declare the EventFlags property
        private ObservableCollection<EventFlagItem> _eventFlags;
        public ObservableCollection<EventFlagItem> EventFlags
        {
            get => _eventFlags;
            set => SetField(ref _eventFlags, value);
        }
    }
    public class EventFlagItem : ViewModelBase, INotifyPropertyChanged
    {
        public ErdHook Hook { get; private set; }

        //private bool _isEventFlag;
        //public bool IsEventFlag
        //{
        //    get => _isEventFlag;
        //    set
        //    {
        //        if (_isEventFlag != value)
        //        {
        //            _isEventFlag = value;
        //            OnPropertyChanged(nameof(IsEventFlag));
        //        }
        //    }
        //}

        private bool? _isEventFlag;
        public bool? IsEventFlag
        {
            get => _isEventFlag;
            set => SetField(ref _isEventFlag, value);
        }
        public int EventID { get; set; }
        public string DisplayText { get; set; }
        public string URL { get; set; } 

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
