using Elden_Ring_Debug_Tool_ViewModels.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using Erd_Tools.Models.System.Dlc;
using PropertyHook;
using Grace = Erd_Tools.Models.Grace;
using GraceViewModel = Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels.GraceViewModel;
using static Elden_Ring_Debug_Tool_ViewModels.Attributes.HotKeyParameterAttribute;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Grace View")]
    public class GraceViewViewModel : ViewModelBase
    {
        public ICollectionView GraceCollectionView { get; set; }
        public ICollectionView HubCollectionView { get; set; }

        public ICommand WarpCommand { get; }
        public ICommand WarpLastCommand { get; }
        public ICommand SetGraceCommand { get; }
        public ICommand ManageAllGraceCommand { get; }
        public ICommand ManageAllHubsCommand { get; }

        // private ObservableCollection<HotKeyViewModel> _graceCommands;
        // public ObservableCollection<HotKeyViewModel> GraceCommands
        // {
        //     get => _graceCommands;
        //     set
        //     {
        //         if (SetField(ref _graceCommands, value))
        //         {
        //             GraceCollectionView.Filter += FilterCommands;
        //             OnPropertyChanged(nameof(GraceCollectionView));
        //         }
        //     }
        // }
        //
        // private MainWindowViewModel _mainWindowViewModel { get; set; }

        internal ErdHook Hook { get; set; }

        public GraceViewViewModel()
        {
            WarpCommand = new WarpCommand(this);
            WarpLastCommand = new WarpLastCommand(this);
            Commands.Add(WarpLastCommand);
            SetGraceCommand = new SetGraceCommand(this);
            ManageAllGraceCommand = new ManageAllGraceCommand(this);
            ManageAllHubsCommand = new ManageAllHubsCommand(this);
        }

        public bool MassChange { get; set; }

        public void UpdateViewModel()
        {
            if (!Setup)
            {
                return;
            }

            if (!MassChange)
            {
                foreach (GraceViewModel grace in SelectedHubViewModel.Graces)
                {
                    grace.Update(Hook.CSFD4VirtualMemoryFlag.IsEventFlagFast(grace.EventFlagID));
                }
            }

            HideDlc = SettingsViewViewModel.Settings.HideDlc;

            LastGraceID = Hook.LastGrace;

            Setup = Hook.Setup;
            Loaded = Hook.Loaded;
        }


        public void ReloadViewModel()
        {
            if (string.IsNullOrWhiteSpace(GraceFilter))
            {
                GraceViewModel? graceViewModel =
                    GraceViewModel.All.FirstOrDefault(g => g.EntityID + 1000 == Hook.LastGrace);

                if (graceViewModel != null)
                {
                    SelectedGraceViewModel = graceViewModel;
                }
            }
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

        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
            Hook.OnSetup += Hook_OnSetup;
        }

        private void Hook_OnSetup(object? sender, PHEventArgs e)
        {
            List<Continent> continents = Hook.GetContinents();
            foreach (Continent continent in continents)
            {
                new ContinentViewModel(continent, Hook);
            }

            GraceCollectionView = CollectionViewSource.GetDefaultView(GraceViewModel.All);
            GraceCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GraceViewModel.Continent)));
            GraceCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GraceViewModel.Hub)));
            GraceCollectionView.Filter += FilerGrace;
            SelectedGraceViewModel = (GraceViewModel)GraceCollectionView.CurrentItem;

            HubCollectionView = CollectionViewSource.GetDefaultView(HubViewModel.All);
            HubCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HubViewModel.Continent)));
            HubCollectionView.Filter += FilerHub;
            SelectedHubViewModel = (HubViewModel)HubCollectionView.CurrentItem;
            HubCollectionView.Refresh();

            OnPropertyChanged(nameof(GraceCollectionView));
            OnPropertyChanged(nameof(HubCollectionView));

            Setup = true;
            //System.Windows.Application.Current.Dispatcher.Invoke(() =>
            //{
            //    foreach (GraceViewModel grace in GraceViewModel.All)
            //    {
            //        grace.Update(Hook.CheckGraceStatus(grace.Grace));
            //    }
            //});
        }

        private bool _quickSelectBonfire;

        public bool QuickSelectBonfire
        {
            get => _quickSelectBonfire;
            set => SetField(ref _quickSelectBonfire, value);
        }

        private int _lastGraceID;

        public int LastGraceID
        {
            get => _lastGraceID;
            set
            {
                if (SetField(ref _lastGraceID, value))
                {
                    int lastGraceID = LastGraceID - 1000;
                    if (lastGraceID != LastGraceViewModel?.EntityID)
                    {
                        GraceViewModel? graceViewModel =
                            GraceViewModel.All.FirstOrDefault(g => g.EntityID == lastGraceID);
                        if (graceViewModel != null)
                        {
                            LastGraceViewModel = graceViewModel;
                        }
                    }
                }
            }
        }

        private GraceViewModel? _lastGraceViewModel;

        public GraceViewModel? LastGraceViewModel
        {
            get => _lastGraceViewModel;
            set => SetField(ref _lastGraceViewModel, value);
        }


        private GraceViewModel _selectedGraceViewModel;

        public GraceViewModel SelectedGraceViewModel
        {
            get => _selectedGraceViewModel;
            set
            {
                if (SetField(ref _selectedGraceViewModel, value) && QuickSelectBonfire)
                {
                    Hook.LastGrace = SelectedGraceViewModel.EntityID + 1000;
                    //LastGraceViewModel = SelectedGraceViewModel;
                }
            }
        }

        private HubViewModel _selectedHubViewModel;

        public HubViewModel SelectedHubViewModel
        {
            get => _selectedHubViewModel;
            set => SetField(ref _selectedHubViewModel, value);
        }

        private bool _hideDlc = true;

        public bool HideDlc
        {
            get => _hideDlc;
            set
            {
                if (SetField(ref _hideDlc, value))
                {
                    GraceCollectionView.Refresh();
                    HubCollectionView.Refresh();
                    ;
                }
            }
        }

        #region Search

        private string _graceFilter = string.Empty;

        public string GraceFilter
        {
            get => _graceFilter;
            set
            {
                if (SetField(ref _graceFilter, value))
                {
                    GraceCollectionView.Refresh();

                    if (!GraceCollectionView.IsEmpty)
                    {
                        GraceCollectionView.MoveCurrentToFirst();
                        SelectedGraceViewModel = (GraceViewModel)GraceCollectionView.CurrentItem;
                    }
                }
            }
        }

        private bool FilerGrace(object obj)
        {
            if (obj is GraceViewModel grace)
            {
                if (HideDlc && grace.Dlc != DlcName.None)
                {
                    return false;
                }

                return grace.Name.Contains(GraceFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        private string _hubFilter = string.Empty;

        public string HubFilter
        {
            get => _hubFilter;
            set
            {
                if (SetField(ref _hubFilter, value))
                {
                    HubCollectionView.Refresh();
                    if (!HubCollectionView.IsEmpty)
                    {
                        HubCollectionView.MoveCurrentToFirst();
                        SelectedHubViewModel = (HubViewModel)HubCollectionView.CurrentItem;
                        return;
                    }
                }
            }
        }

        private bool FilerHub(object obj)
        {
            if (obj is HubViewModel hub)
            {
                if (HideDlc && hub.Dlc != DlcName.None)
                {
                    return false;
                }

                return hub.Name.Contains(HubFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        #endregion
    }
}