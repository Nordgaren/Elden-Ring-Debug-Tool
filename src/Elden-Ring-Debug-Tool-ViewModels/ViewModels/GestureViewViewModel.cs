using Elden_Ring_Debug_Tool_ViewModels.Commands.GestureView;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using Erd_Tools.Models.Game;
using PropertyHook;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Gesture View")]
    public class GestureViewViewModel : ViewModelBase
    {
        private ICollectionView _gestureCollectionView;
        public ICollectionView GestureCollectionView { get => _gestureCollectionView;
            set => SetField(ref _gestureCollectionView, value);
        }       
        
        private ObservableCollection<GestureViewModel> _gestures;
        public ObservableCollection<GestureViewModel> Gestures { get => _gestures;
            set => SetField(ref _gestures, value);
        }

        public ICommand SetGestureCommand { get; }

        internal ErdHook Hook { get; set; }

        public GestureViewViewModel()
        {
            SetGestureCommand = new SetGestureCommand(this);
        }
        
        public void UpdateViewModel() {
            if (!Setup)
            {
                return;
            }
            
            foreach (GestureViewModel gesture in GestureCollectionView)
            {
                gesture.Enabled = Hook.GestureGameData.CheckGesture(gesture.Id);
            }
        }


        public void ReloadViewModel()
        {

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
            List<Gesture> gestures = Hook.GestureGameData.GetGestures();
            Gestures = new ObservableCollection<GestureViewModel>();

            foreach (Gesture gesture in gestures)
            {
                Gestures.Add(new GestureViewModel(gesture));
            }
            Setup = true;
            GestureCollectionView = CollectionViewSource.GetDefaultView(Gestures);
            GestureCollectionView.Filter += FilerGestures;
        }       
        
        private string _gestureFilter = string.Empty;
        public string GestureFilter
        {
            get => _gestureFilter;
            set
            {
                if (SetField(ref _gestureFilter, value))
                {
                    GestureCollectionView.Refresh();

                    // if (!GestureCollectionView.IsEmpty)
                    // {
                    //     GestureCollectionView.MoveCurrentToFirst();
                    // }
                }
            }
        }
        
        private bool FilerGestures(object obj)
        {
            if (obj is GestureViewModel gesture)
            {
                return gesture.Name.Contains(GestureFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
    }
}