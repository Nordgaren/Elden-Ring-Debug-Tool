using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Face View")]
    public class FaceViewViewModel : ViewModelBase
    {
        internal ErdHook Hook { get; private set; }
        public ICommand SaveFaceDataCommand { get; set; }
        
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

        public FaceViewViewModel()
        {

        }
        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
            SaveFaceDataCommand = new SaveFaceDataCommand(this);
        }
        private bool _reading = false;
        public void UpdateViewModel()
        {
            _reading = true;
            if (Hook.PlayerGameData.Resolve() != IntPtr.Zero)
            {
                Bytes = Hook.PlayerGameData.FaceData;
            }

            Setup = Hook.Setup;
            Loaded = Hook.Loaded;
            _reading = false;
        }
        
        private string _hexString = "";
        public string HexString
        {
            get => _hexString;
            set => SetField(ref _hexString, value);
        }

        private byte[] _bytes;
        public byte[] Bytes
        {
            get => _bytes;
            set
            {
                if (SetField(ref _bytes, value))
                {
                    HexString = string.Join(" ", _bytes.Select(x => x.ToString("X2")));
                }
            }
        }
    }
}
