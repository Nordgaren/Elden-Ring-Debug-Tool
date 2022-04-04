using PropertyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    internal class ERHook : PHook, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private PHPointer GameDataManSetup;
        private PHPointer GameDataMan;
        private PHPointer PlayerGameData;
        private PHPointer PlayerInventory;

        public ERHook(int refreshInterval, int minLifetime) :
            base(refreshInterval, minLifetime, p => p.MainWindowTitle == "ELDEN RING™")
        {
            GameDataManSetup = RegisterAbsoluteAOB(EROffsets.GameDataManSetupAoB);
            OnHooked += ERHook_OnHooked;
        }
        private void ERHook_OnHooked(object? sender, PHEventArgs e)
        {
            GameDataMan = CreateBasePointer(BasePointerFromSetupPointer(GameDataManSetup));
            PlayerGameData = CreateChildPointer(GameDataMan, (int)EROffsets.PlayerGameDataOffset);
            PlayerInventory = CreateChildPointer(PlayerGameData, (int)EROffsets.PlayerInventoryOffset);
        }
        public IntPtr BasePointerFromSetupPointer(PHPointer pointer)
        {
            var readInt = pointer.ReadInt32(EROffsets.BasePtrOffset1);
            return pointer.ReadIntPtr(readInt + EROffsets.BasePtrOffset2);
        }

        public void Update()
        {
            if (!Hooked)
                return;
            var gdm = GameDataMan.Resolve();
            OnPropertyChanged(nameof(PlayerInventoryFirstInt));
        }

        public uint? PlayerInventoryFirstInt => PlayerInventory?.ReadUInt32(0x0);

    }
}
