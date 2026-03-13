using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models.CSFD4;
using Org.BouncyCastle.Asn1;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class MiscViewViewModel : ViewModelBase
    {
        public ErdHook Hook { get; private set; }
        public ICollectionView EventWatchCollectionView => CollectionViewSource.GetDefaultView(EventWatchList);
        private ObservableCollection<EventViewModel> _eventWatchList;

        public ObservableCollection<EventViewModel> EventWatchList
        {
            get => _eventWatchList;
            set
            {
                if (SetField(ref _eventWatchList, value))
                {
                    OnPropertyChanged(nameof(EventWatchCollectionView));
                    // EventWaCollectionView.Filter += FilterEvents;
                }
            }
        }

        public ICommand EnableEventFlag { get; }
        public ICommand DisableEventFlag { get; }
        public ICommand CheckEventFlag { get; }
        public ICommand AddEventWatch { get; }
        public ICommand RemoveEventWatch { get; }

        public MiscViewViewModel()
        {
            EnableEventFlag = new EnableEventCommand(this);
            DisableEventFlag = new DisableEventCommand(this);
            CheckEventFlag = new IsEventCommand(this);
            AddEventWatch = new AddEventWatchCommand(this);
            RemoveEventWatch = new RemoveEventWatchCommand(this);
            if (File.Exists("Resources\\WatchList.txt"))
            {
                string jsonString = File.ReadAllText("Resources\\WatchList.txt");
                EventWatchList =
                    System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<EventViewModel>>(jsonString) ??
                    new ObservableCollection<EventViewModel>();
            }
            else
            {
                EventWatchList = new ObservableCollection<EventViewModel>();
            }

            EventWatchList.CollectionChanged += MiscViewViewModel_PropertyChanged;
        }

        public void UpdateViewModel()
        {
            if (!IsActiveView) return;
            
            Loaded = Hook.Loaded;
            Setup = Hook.Setup;
            // foreach (EventViewModel eventViewModel in EventWatchList)
            // {
            //     uint value = 0;
            //     for (uint e = 0; e < eventViewModel.Bits; e++)
            //     {
            //         value |= Hook.CSFD4VirtualMemoryFlag.IsEventFlagFast(eventViewModel.EventId + e, 1) << (int)e;
            //     }
            //
            //     eventViewModel.Value = value;
            // }
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

        private uint _eventFlag;

        public uint EventFlag
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

        private uint _watchFlag;

        public uint WatchFlag
        {
            get => _watchFlag;
            set
            {
                if (SetField(ref _watchFlag, value))
                {
                    EventWatch.EventId = value;
                }
            }
        }

        private uint _bits = 1;

        public uint Bits
        {
            get => _bits;
            set
            {
                if (SetField(ref _bits, value))
                {
                    EventWatch.Bits = value;
                }
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                if (SetField(ref _description, value))
                {
                    EventWatch.Description = value;
                }
            }
        }

        private EventWatch _eventWatch = new EventWatch(0);

        public EventWatch EventWatch
        {
            get => _eventWatch;
            set => SetField(ref _eventWatch, value);
        }

        private void MiscViewViewModel_PropertyChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach(EventViewModel oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= EventViewModel_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach(EventViewModel newItem in e.NewItems)
                {
                    newItem.PropertyChanged += EventViewModel_PropertyChanged;
                }
            }

            string jsonString = System.Text.Json.JsonSerializer.Serialize(_eventWatchList);
            File.WriteAllText("Resources\\WatchList.txt", jsonString);
        }

        private void EventViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                return;
            }
            
            string jsonString = System.Text.Json.JsonSerializer.Serialize(_eventWatchList);
            File.WriteAllText("Resources\\WatchList.txt", jsonString);
        }
        

    }
}