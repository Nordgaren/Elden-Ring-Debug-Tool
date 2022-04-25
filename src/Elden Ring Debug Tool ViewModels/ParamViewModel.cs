using Elden_Ring_Debug_Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_ViewModels
{
    public class ParamViewModel : ViewModelBase
    {
        private ERHook Hook;
        public ParamViewModel(ERHook hook)
        {
            Hook = hook;
            _params = new ObservableCollection<ERParam>(new List<ERParam>());
            ParamCollectionView = CollectionViewSource.GetDefaultView(_params);

            ParamCollectionView.Filter = FilterParams;
        }

        private readonly ObservableCollection<ERParam> _params;

        public ICollectionView ParamCollectionView  { get; }
        //Param search not yet implimented in UI
        private string _paramSearchTest = string.Empty;
        public string ParamSearchText
        {
            get
            {
                return _paramSearchTest;
            }
            set
            {
                _paramSearchTest = value;
                OnPropertyChanged();
            }
        }

        private bool _setup;
        public bool Setup
        {
            get
            {
                return _setup;
            }
            set
            {
                _setup = value;
                SetField(ref _setup, value);
                OnPropertyChanged(nameof(Setup));
            }
        }

        public ERParam SelectedParam { get; set; }

        public void AddParams()
        {
            foreach (var p in Hook.GetParams())
            {
                _params.Add(p);
            }
        }

        private bool FilterParams(object obj)
        {
            if (obj is ERParam param)
            {
                return param.Type.Contains(ParamSearchText, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public void UpdateView()
        {
            Setup = Hook.Setup;
        }

    }
}
