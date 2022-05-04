using Erd_Tools;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Param View")]
    public class ParamViewViewModel : ViewModelBase
    {
        public string ParamSavePath => $@"{Directory.GetParent(Hook.Process.MainModule.FileName).Parent.FullName}\capture\param";

        internal ERHook Hook { get; set; }
        private readonly ObservableCollection<ERParamViewModel> _params;

        public ICollectionView ParamCollectionView { get; }
        public ICollectionView RowCollectionView => CollectionViewSource.GetDefaultView(SelectedParam?.Rows);
        public ICollectionView FieldCollectionView => CollectionViewSource.GetDefaultView(SelectedParam?.Fields);

        public ICommand SaveParamCommand { get; set; }
        public ICommand ResetParamCommand { get; set; }
        public ICommand OpenParamCaptureFolderCommand { get; set; }

        public ParamViewViewModel()
        {
            _params = new ObservableCollection<ERParamViewModel>(new List<ERParamViewModel>());
            ParamCollectionView = CollectionViewSource.GetDefaultView(_params);
            SaveParamCommand = new SaveParamCommand(this);
            ResetParamCommand = new ResetParamCommand(this);
            OpenParamCaptureFolderCommand = new OpenParamCaptureFolderCommand(this);

            Commands.Add(SaveParamCommand);
            Commands.Add(ResetParamCommand);
            Commands.Add(OpenParamCaptureFolderCommand);
        }

        public void InitViewModel(ERHook hook)
        {
            Hook = hook;
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
        }

        private void Hook_OnUnhooked(object? sender, PropertyHook.PHEventArgs e)
        {
            Setup = false;
        }
        private void Hook_OnSetup(object? sender, PropertyHook.PHEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (ERParam p in Hook.Params)
                {
                    _params.Add(new ERParamViewModel(this, p));
                }
                ParamCollectionView.Filter = FilterParams;

                if (_params.Count > 0)
                    SelectedParam = _params[0];

            });
        }

        private bool _setup;
        public bool Setup
        {
            get => _setup;
            set => SetField(ref _setup, value);
        }

        public void UpdateViewModel()
        {
            Setup = Hook.Setup;
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
                    OnPropertyChanged(nameof(FieldCollectionView));
                    FieldCollectionView.Filter = FilterFields;
                }
            }
        }

        private RowViewModel _selectedRow;
        public RowViewModel SelectedRow
        {
            get
            {
                return _selectedRow;
            }
            set
            {
                SetField(ref _selectedRow, value);
            }
        }

        #region Search
        //Param search not yet implimented in UI
        private string _paramFilter = string.Empty;
        public string ParamFilter
        {
            get => _paramFilter;
            set
            {
                SetField(ref _paramFilter, value);
                ParamCollectionView.Refresh();
            }
        }
        private bool FilterParams(object obj)
        {
            if (obj is ERParamViewModel param)
            {
                return param.Type.Contains(ParamFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        private string _rowFilter = string.Empty;
        public string RowFilter
        {
            get => _rowFilter;
            set
            {
                SetField(ref _rowFilter, value);
                RowCollectionView.Refresh();
            }
        }
        private bool FilterRows(object obj)
        {
            if (obj is RowViewModel row)
            {
                return row.Name.Contains(RowFilter, StringComparison.InvariantCultureIgnoreCase) || row.ID.ToString().Contains(RowFilter);
            }

            return false;
        }

        private string _fieldFilter = string.Empty;
        public string FieldFilter
        {
            get => _fieldFilter;
            set
            {
               if (SetField(ref _fieldFilter, value))
                {
                    FieldCollectionView.Refresh();
                };
            }
        }
        private bool FilterFields(object obj)
        {
            if (obj is FieldViewModel field)
            {
                return field.InternalName.Contains(FieldFilter, StringComparison.InvariantCultureIgnoreCase) 
                    || field.DisplayName.ToString().Contains(FieldFilter, StringComparison.InvariantCultureIgnoreCase) 
                    || field.StringValue.Contains(FieldFilter);
            }

            return false;
        }

        #endregion

    }
}
