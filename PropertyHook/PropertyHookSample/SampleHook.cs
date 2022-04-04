using PropertyHook;
using System;

namespace PropertyHookSample
{
    public class SampleHook : PHook
    {
        private PHPointer WorldChrBase;
        private PHPointer ChrData1;
        private PHPointer ChrData2;

        public SampleHook() : base(5000, 5000, p => p.ProcessName == "DarkSoulsRemastered")
        {
            WorldChrBase = CreateBasePointer((IntPtr)0x141D151B0, 0);
            ChrData1 = CreateChildPointer(WorldChrBase, 0x68);
            ChrData2 = RegisterRelativeAOB("48 8B 05 ? ? ? ? 48 85 C0 ? ? F3 0F 58 80 AC 00 00 00", 3, 7, 0, 0x10);
        }
        public bool DeathCam
        {
            get => WorldChrBase.ReadBoolean(0x70);
            set => WorldChrBase.WriteBoolean(0x70, value);
        }

        public int Health
        {
            get => ChrData1.ReadInt32(0x3E8);
            set => ChrData1.WriteInt32(0x3E8, value);
        }

        public int Stamina
        {
            get => ChrData1.ReadInt32(0x3F8);
            set => ChrData1.WriteInt32(0x3F8, value);
        }

        public Stats GetStats()
        {
            Stats stats;
            stats.Vitality = ChrData2.ReadInt32(0x40);
            stats.Attunement = ChrData2.ReadInt32(0x48);
            stats.Endurance = ChrData2.ReadInt32(0x50);
            // etc
            return stats;
        }
    }

    public struct Stats
    {
        public int Vitality;
        public int Attunement;
        public int Endurance;
    }
}
