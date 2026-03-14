using Erd_Tools.Models;
using System.Collections.ObjectModel;
using SoulsFormats;
using System.Windows;
using static Erd_Tools.Models.Param;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ParamViewModel : ViewModelBase
    {
        public ParamViewViewModel ParamViewerViewModel { get; }
        public Param Param { get; }
        public int Offset => Param.Offset;
        public string Name => Param.Name;
        public string Type => Param.Type;
        public int Length => Param.Length;
        public byte[] Bytes => Param.Bytes;
        private ObservableCollection<RowViewModel> _rows;
        public ObservableCollection<RowViewModel> Rows 
        { 
            get => _rows; 
            private set => SetField(ref _rows, value); 
        }
        private ObservableCollection<FieldViewModel> _fields;
        public ObservableCollection<FieldViewModel> Fields 
        { 
            get => _fields; 
            private set => SetField(ref _fields, value); 
        }
        public Dictionary<int, string> NameDictionary => Param.NameDictionary;
        public Dictionary<int, int> OffsetDict => Param.OffsetDict;
        public int RowLength => Param.RowLength;
        private bool _isLoaded;
        private bool _isLoading;

        public ParamViewModel(ParamViewViewModel paramViewerViewModel, Param param)
        {
            ParamViewerViewModel = paramViewerViewModel;
            Param = param;
            Rows = new ObservableCollection<RowViewModel>();
            Fields = new ObservableCollection<FieldViewModel>();

        }
        
        public async Task LoadDataAsync()
        {
            // Prevent multiple triggers from loading the same param simultaneously
            if (_isLoaded || _isLoading) return;
            _isLoading = true;

            // Tell the underlying model to parse the memory (Runs on background thread)
            await Param.LoadAsync();

            // Build the view models on a background thread so the UI doesn't freeze
            List<RowViewModel> tempRows = new();
            List<FieldViewModel> tempFields = new();

            await Task.Run(() =>
            {
                foreach (Row row in Param.Rows)
                {
                    tempRows.Add(new RowViewModel(row));
                }

                foreach (Field field in Param.Fields)
                {
                    tempFields.Add(GetFieldViewModel(field));
                }
            });

            // Reassign the collections on the main UI thread. 
            // Reassigning is WAY faster than doing Rows.Add() thousands of times in a loop!
            Application.Current.Dispatcher.Invoke(() =>
            {
                Rows = new ObservableCollection<RowViewModel>(tempRows);
                Fields = new ObservableCollection<FieldViewModel>(tempFields);
            });

            // Notify the UI that the underlying properties are now populated
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(Bytes));
            OnPropertyChanged(nameof(NameDictionary));
            OnPropertyChanged(nameof(OffsetDict));
            OnPropertyChanged(nameof(RowLength));

            _isLoaded = true;
            _isLoading = false;
        }

        private FieldViewModel GetFieldViewModel(Field field)
        {
            if (field is NumericField numField)
            {
                switch (field.Type)
                {
                    case PARAMDEF.DefType.s8:
                        return new NumericSignedViewModel<sbyte>(ParamViewerViewModel, numField);
                    case PARAMDEF.DefType.dummy8:
                    case PARAMDEF.DefType.u8:
                        return new NumericViewModel<byte>(ParamViewerViewModel, numField);
                    case PARAMDEF.DefType.s16:
                        return new NumericSignedViewModel<short>(ParamViewerViewModel, numField);
                    case PARAMDEF.DefType.u16:
                        return new NumericViewModel<ushort>(ParamViewerViewModel, numField);
                    case PARAMDEF.DefType.s32:
                        return new NumericSignedViewModel<int>(ParamViewerViewModel, numField);
                    case PARAMDEF.DefType.u32:
                        return new NumericViewModel<uint>(ParamViewerViewModel, numField);
                    default:
                        throw new Exception($"No view model found for NumericField {field.InternalName}");
                }
            }

            if (field is SingleField floatField)
            {
                return new SingleFieldViewModel(ParamViewerViewModel, floatField);
            }

            if (field is FixedStr fixedStr)
            {
                return new FixedStrViewModel(ParamViewerViewModel, fixedStr);
            }

            if (field is PartialByteField partialByteField)
            {
                return new PartialByteViewModel(ParamViewerViewModel, partialByteField);
            }

            if (field is PartialUShortField partialUShortField)
            {
                return new PartialUShortViewModel(ParamViewerViewModel, partialUShortField);
            }

            if (field is PartialUIntField partialUIntField)
            {
                return new PartialUintViewModel(ParamViewerViewModel, partialUIntField);
            }

            if (field is BitField bitField)
            {
                return new BitFieldViewModel(ParamViewerViewModel, bitField);
            }

            throw new Exception($"No view model found for {field.InternalName}");
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
