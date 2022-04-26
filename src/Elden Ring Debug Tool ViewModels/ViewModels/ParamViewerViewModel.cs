using Elden_Ring_Debug_Tool;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class ParamViewerViewModel : ViewModelBase
    {
        public string ParamSavePath => $@"{Directory.GetParent(_hook.Process.MainModule.FileName).Parent.FullName}\capture\param";

        private ERHook _hook { get; set; }
        private readonly ObservableCollection<ERParamViewModel> _params;
        private ObservableCollection<ERParam.Row> _rows;

        public ICollectionView ParamCollectionView { get; }
        public ICollectionView RowCollectionView => CollectionViewSource.GetDefaultView(_selectedParam?.Rows);
        //public ICollectionView FieldCollectionView => CollectionViewSource.GetDefaultView(_selectedParam?.Rows);

        public ICommand SaveParamCommand { get; set; }
        public ICommand ResetParamCommand { get; set; }
        public ICommand OpenParamCaptureFolderCommand { get; set; }

        public ParamViewerViewModel()
        {
            _params = new ObservableCollection<ERParamViewModel>(new List<ERParamViewModel>());
            ParamCollectionView = CollectionViewSource.GetDefaultView(_params);
            _rows = new ObservableCollection<ERParam.Row>(new List<ERParam.Row>());
        }

        public void SetHook(ERHook hook)
        {
            _hook = hook;
            SaveParamCommand = new SaveParamCommand(this, _hook);
            ResetParamCommand = new ResetParamCommand(this, _hook);
            OpenParamCaptureFolderCommand = new OpenParamCaptureFolderCommand(this, _hook);
        }
        public void AddParams()
        {
            foreach (var p in _hook.GetParams())
            {
                _params.Add(new ERParamViewModel(p));
            }
            ParamCollectionView.Filter = FilterParams;
            SelectedParam = _params[0];
            RowCollectionView.Filter = FilterRows;
            //FieldCollectionView.Filter = FilterRows;
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
                SetField(ref _setup, value);

            }
        }


        private ERParamViewModel _selectedParam;
        public ERParamViewModel SelectedParam
        {
            get
            {
                return _selectedParam;
            }
            set
            {
                if (SetField(ref _selectedParam, value))
                {
                    OnPropertyChanged(nameof(RowCollectionView));
                    RowCollectionView.Filter = FilterRows;
                }
            }
        }

        private ERParam.Row _selectedRow;
        public ERParam.Row SelectedRow
        {
            get
            {
                return _selectedRow;
            }
            set
            {
                if (SetField(ref _selectedRow, value)) return;
                    //OnPropertyChanged(nameof(FieldCollectionView));
            }
        }

        public void UpdateView()
        {
            Setup = _hook.Setup;
        }

        #region Search
        //Param search not yet implimented in UI
        private string _paramFilter = string.Empty;
        public string ParamFilter
        {
            get
            {
                return _paramFilter;
            }
            set
            {
                SetField(ref _paramFilter, value);
            }
        }
        private bool FilterParams(object obj)
        {
            if (obj is ERParamViewModel param)
            {
                return param.Type.Contains(ParamFilter, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private string _rowFilter = string.Empty;
        public string RowFilter
        {
            get
            {
                return _rowFilter;
            }
            set
            {
                SetField(ref _rowFilter, value);
                RowCollectionView.Refresh();
            }
        }
        private bool FilterRows(object obj)
        {
            if (obj is ERParam.Row row)
            {
                return row.Name.Contains(RowFilter, StringComparison.OrdinalIgnoreCase) || row.ID.ToString().Contains(RowFilter);
            }

            return false;
        }

        private string _fieldFilter = string.Empty;
        public string FieldFilter
        {
            get
            {
                return _fieldFilter;
            }
            set
            {
                SetField(ref _fieldFilter, value);
                //FieldCollectionView.Refresh();
            }
        }
        private bool FilterFields(object obj)
        {
            if (obj is FieldViewModel field)
            {
                return field.InternalName.Contains(FieldFilter, StringComparison.OrdinalIgnoreCase) || field.DisplayName.ToString().Contains(FieldFilter);
            }

            return false;
        }

        #endregion


    }
}
