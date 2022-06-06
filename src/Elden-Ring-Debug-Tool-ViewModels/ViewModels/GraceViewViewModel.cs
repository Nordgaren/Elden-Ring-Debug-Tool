using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using Grace = Erd_Tools.Models.Grace;
using GraceViewModel = Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels.GraceViewModel;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class GraceViewViewModel : ViewModelBase
    {

        private ObservableCollection<GraceViewModel> _graceCollection;

        public ObservableCollection<GraceViewModel> GraceCollection
        {
            get => _graceCollection;
            set => SetField(ref _graceCollection, value);
        }

        private ObservableCollection<HubViewModel> _hubCollection;

        public ObservableCollection<HubViewModel> HubCollection
        {
            get => _hubCollection;
            set => SetField(ref _hubCollection, value);
        }

        internal ErdHook Hook { get; set; }


        public GraceViewViewModel()
        {
        }

        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
            foreach (Continent continent in Continent.Continents)
            {
                new ContinentViewModel(continent);
            }

            GraceCollection = GraceViewModel.All;
            HubCollection = HubViewModel.All;;
            //WarpCollectionView = CollectionViewSource.GetDefaultView(GraceViewModel.All);
        }


        private GraceViewModel _selectedGraceViewModel;

        public GraceViewModel SelectedGraceViewModel
        {
            get => _selectedGraceViewModel;
            set => SetField(ref _selectedGraceViewModel, value);
        }

        private HubViewModel _selectedHubViewModel;

        public HubViewModel SelectedHubViewModel
        {
            get => _selectedHubViewModel;
            set => SetField(ref _selectedHubViewModel, value);
        }
        #region Search

        private string _warpFilter = string.Empty;
        public string WarpFilter
        {
            get => _warpFilter;
            set
            {
                if (SetField(ref _warpFilter, value))
                {
                    //ItemsCollectionView.Refresh();
                }
            }
        }

        private bool FilerWarp(object obj)
        {
            if (obj is object item)
            {
                //return item.Name.Contains(GraceFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        private string _graceFilter = string.Empty;
        public string GraceFilter
        {
            get => _graceFilter;
            set
            {
                if (SetField(ref _graceFilter, value))
                {
                    //ItemsCollectionView.Refresh();
                }
            }
        }

        private bool FilerGrace(object obj)
        {
            if (obj is object item)
            {
                //return item.Name.Contains(GraceFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
        #endregion
    }
}
