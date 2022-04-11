using PropertyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Infusion = Elden_Ring_Debug_Tool.ERWeapon.Infusion;
using WeaponType = Elden_Ring_Debug_Tool.ERWeapon.WeaponType;

namespace Elden_Ring_Debug_Tool
{
    public class ERHook : PHook, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private PHPointer GameDataManSetup { get; set; }
        private PHPointer GameDataMan { get; set; }
        private PHPointer PlayerGameData { get; set; }
        private PHPointer SoloParamRepositorySetup { get; set; }
        private PHPointer SoloParamRepository { get; set; }
        public PHPointer EquipParamWeapon { get; set; }
        public PHPointer EquipParamGem { get; set; }
        public event EventHandler<PHEventArgs> OnSetup;
        private void RaiseOnSetup()
        {
            OnSetup?.Invoke(this, new PHEventArgs(this));
        }
        public List<PHPointer> Params;
        //private PHPointer DurabilityAddr { get; set; }
        //private PHPointer DurabilitySpecialAddr { get; set; }
        public bool Loaded => PlayerGameData != null ?  PlayerGameData.Resolve() != IntPtr.Zero : false;
        private bool Setup = false;
        public ERHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector)
            : base(refreshInterval, minLifetime, processSelector)
        {
            OnHooked += ERHook_OnHooked;
            GameDataManSetup = RegisterAbsoluteAOB(EROffsets.GameDataManSetupAoB);
            SoloParamRepositorySetup = RegisterAbsoluteAOB(EROffsets.SoloParamRepositorySetupAoB);
        }

        internal PHPointer GetParamPointer(int[] offset)
        {
            return CreateChildPointer(SoloParamRepository, offset);
        }

        private void ERHook_OnHooked(object? sender, PHEventArgs e)
        {
            Params = new List<PHPointer>();
            GameDataMan = CreateBasePointer(BasePointerFromSetupPointer(GameDataManSetup));
            PlayerGameData = CreateChildPointer(GameDataMan, EROffsets.PlayerGameData);

            SoloParamRepository = CreateBasePointer(BasePointerFromSetupPointer(SoloParamRepositorySetup));
            EquipParamWeapon = CreateChildPointer(SoloParamRepository, EROffsets.EquipParamWeaponOffset1, EROffsets.EquipParamWeaponOffset2, EROffsets.EquipParamWeaponOffset3);
            EquipParamGem = CreateChildPointer(SoloParamRepository, EROffsets.EquipParamGemOffset1, EROffsets.EquipParamGemOffset2, EROffsets.EquipParamGemOffset3);
            var bytes = new byte[0];
            EquipParamWeaponOffsetDict = BuildOffsetDictionary(EquipParamWeapon, "EQUIP_PARAM_WEAPON_ST", ref bytes);
            EquipParamWeaponBytes = bytes;
            EquipParamGemOffsetDict = BuildOffsetDictionary(EquipParamGem, "EQUIP_PARAM_GEM_ST", ref bytes);
            EquipParamGemBytes = bytes;
            Params.Add(EquipParamWeapon);
            Params.Add(EquipParamGem);
            GetParams();
            Setup = true;
            RaiseOnSetup();
        }
        public void Update()
        {
            if (!Setup)
                return;

            OnPropertyChanged(nameof(Loaded));
            OnPropertyChanged(nameof(Dagger));
            OnPropertyChanged(nameof(SwordNormal));
            OnPropertyChanged(nameof(SwordLarge));
            OnPropertyChanged(nameof(SwordGigantic));
            OnPropertyChanged(nameof(SabreNormal));
            OnPropertyChanged(nameof(SabreLarge));
            OnPropertyChanged(nameof(katana));
            OnPropertyChanged(nameof(SwordDoulbeEdge));
            OnPropertyChanged(nameof(SwordPierce));
            OnPropertyChanged(nameof(RapierHeavy));
            OnPropertyChanged(nameof(AxeNormal));
            OnPropertyChanged(nameof(AxeLarge));
            OnPropertyChanged(nameof(HammerNormal));
            OnPropertyChanged(nameof(HammerLarge));
            OnPropertyChanged(nameof(Flail));
            OnPropertyChanged(nameof(SpearNormal));
            OnPropertyChanged(nameof(SpearLarge));
            OnPropertyChanged(nameof(SpearHeavy));
            OnPropertyChanged(nameof(SpearAxe));
            OnPropertyChanged(nameof(Sickle));
            OnPropertyChanged(nameof(Knuckle));
            OnPropertyChanged(nameof(Claw));
            OnPropertyChanged(nameof(Whip));
            OnPropertyChanged(nameof(Axhammerlarge));
            OnPropertyChanged(nameof(BowSmall));
            OnPropertyChanged(nameof(BowNormal));
            OnPropertyChanged(nameof(BowLarge));
            OnPropertyChanged(nameof(Clossbow));
            OnPropertyChanged(nameof(Ballista));
            OnPropertyChanged(nameof(Staff));
            OnPropertyChanged(nameof(Talisman));
            OnPropertyChanged(nameof(ShieldSmall));
            OnPropertyChanged(nameof(ShieldNormal));
            OnPropertyChanged(nameof(SheildLarge));
            OnPropertyChanged(nameof(Torch));
        }
        private void GetParams()
        {
            foreach (var category in ERItemCategory.All)
            {
                GetProperties(category);
            }

            foreach (var gem in ERGem.Gems)
            {
                GetInfusions(gem);
                GetWeapons(gem);
            }
        }

        private void GetWeapons(ERGem gem)
        {
            gem.WeaponTypes = new List<WeaponType>();
        }

        private void GetInfusions(ERGem gem)
        {
            gem.Infusions = new List<Infusion>();
            var bitField = BitConverter.ToInt32(EquipParamGemBytes, EquipParamGemOffsetDict[gem.ID] + (int)EROffsets.EquipParamGem.canMountWep_Dagger);
            //if (bitField == 0)
            //    return;



            //for (int i = 1; i < WeaponType.; i++)
            //{
            //    if ((bitField & (1 << i)) != 0)
            //        infusions.Add(DS2SInfusion.Infusions[i]);
            //}
        }

        private void GetProperties(ERItemCategory category)
        {
            foreach (var weapon in category.Weapons)
            {
                if (!EquipParamWeaponOffsetDict.ContainsKey(weapon.ID))
                {
                    Debug.WriteLine($"{weapon.ID} {weapon.Name}");
                    continue;
                }
                weapon.Unique = BitConverter.ToInt32(EquipParamWeaponBytes, EquipParamWeaponOffsetDict[weapon.ID] + (int)EROffsets.EquipParamWeapon.MaterialSetID) == 2200;
                weapon.SwordArtId = BitConverter.ToInt32(EquipParamWeaponBytes, EquipParamWeaponOffsetDict[weapon.ID] + (int)EROffsets.EquipParamWeapon.SwordArtsParamId);
                weapon.Type = (ERWeapon.WeaponType)BitConverter.ToInt32(EquipParamWeaponBytes, EquipParamWeaponOffsetDict[weapon.ID] + (int)EROffsets.EquipParamWeapon.WepType);
            }
        }

        public Dictionary<int, int> EquipParamWeaponOffsetDict { get; private set; }
        private byte[] EquipParamWeaponBytes { get; set; }
        public Dictionary<int, int> EquipParamGemOffsetDict { get; private set; }
        private byte[] EquipParamGemBytes { get; set; }
        private Dictionary<int, int> BuildOffsetDictionary(PHPointer pointer, string expectedParamName, ref byte[] paramBytes)
        {
            var dictionary = new Dictionary<int, int>();
            var nameOffset = pointer.ReadInt32((int)EROffsets.Param.NameOffset);
            var paramName = pointer.ReadString(nameOffset, Encoding.UTF8, 0x18);
            if (paramName != expectedParamName)
                throw new InvalidOperationException($"Incorrect Param Pointer: {paramName} should be {expectedParamName}");

            paramBytes = pointer.ReadBytes((int)EROffsets.Param.TotalParamLength, (uint)nameOffset);
            var tableLength = pointer.ReadInt32((int)EROffsets.Param.TableLength);
            var param = 0x40;
            var paramID = 0x0;
            var paramOffset = 0x8;
            var nextParam = 0x18;

            while (param < tableLength)
            {
                var itemID = pointer.ReadInt32(param + paramID);
                var itemParamOffset = pointer.ReadInt32(param + paramOffset);
                dictionary.Add(itemID, itemParamOffset);

                param += nextParam;
            }

            return dictionary;
        }

        public void RestoreParams()
        {
            if (!Setup)
                return;

            EquipParamWeapon.WriteBytes((int)EROffsets.Param.TotalParamLength, EquipParamWeaponBytes);
            EquipParamGem.WriteBytes((int)EROffsets.Param.TotalParamLength, EquipParamGemBytes);
        }
        public IntPtr BasePointerFromSetupPointer(PHPointer pointer)
        {
            var readInt = pointer.ReadInt32(EROffsets.BasePtrOffset1);
            return pointer.ReadIntPtr(readInt + EROffsets.BasePtrOffset2);
        }
        public int Level => PlayerGameData.ReadInt32((int)EROffsets.Player.Level);
        public string LevelString => PlayerGameData?.ReadInt32((int)EROffsets.Player.Level).ToString() ?? "";
        public byte ArmStyle
        {
            get => PlayerGameData.ReadByte((int)EROffsets.Weapons.ArmStyle);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteByte((int)EROffsets.Weapons.ArmStyle, value);
            }
        }
        public int CurrWepSlotOffsetLeft
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.CurrWepSlotOffsetLeft);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.CurrWepSlotOffsetLeft, value);
            }
        }
        public int CurrWepSlotOffsetRight
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.CurrWepSlotOffsetRight);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.CurrWepSlotOffsetRight, value);
            }
        }
        public int RHandWeapon1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.RHandWeapon1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.RHandWeapon1, value);
            }
        }
        public int RHandWeapon2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.RHandWeapon2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.RHandWeapon2, value);
            }
        }
        public int RHandWeapon3
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.RHandWeapon3);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.RHandWeapon3, value);
            }
        }
        public int LHandWeapon1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.LHandWeapon1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.LHandWeapon1, value);
            }
        }
        public int LHandWeapon2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.LHandWeapon2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.LHandWeapon2, value);
            }
        }
        public int LHandWeapon3
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.LHandWeapon3);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.LHandWeapon3, value);
            }
        }
        public int Arrow1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.Arrow1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.Arrow1, value);
            }
        }
        public int Arrow2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.Arrow2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.Arrow2, value);
            }
        }
        public int Bolt1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.Bolt1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.Bolt1, value);
            }
        }
        public int Bolt2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.Weapons.Bolt2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.Weapons.Bolt2, value);
            }
        }

        private int OGRHandWeapon1 { get; set; }
        private PHPointer OGRHandWeapon1Param
        {
            get => CreateBasePointer(EquipParamWeapon.Resolve() + EquipParamWeaponOffsetDict[OGRHandWeapon1]);
        }
        private int OGRHandWeapon1SwordArtID
        {
            get => OGRHandWeapon1Param?.ReadInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId) ?? 0;
            set => OGRHandWeapon1Param?.WriteInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId, value);
        }
        private int OGLHandWeapon1 { get; set; }
        private PHPointer OGLHandWeapon1Param
        {
            get => CreateBasePointer(EquipParamWeapon.Resolve() + EquipParamWeaponOffsetDict[OGLHandWeapon1]);
        }
        private int OGLHandWeapon1SwordArtID
        {
            get => OGLHandWeapon1Param?.ReadInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId) ?? 0;
            set => OGLHandWeapon1Param?.WriteInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId, value);
        }
        public int AshOfWarID { get; set; } = 40100;
        public uint AshOfWarBitfield1
        { 
            get => EquipParamGem?.ReadUInt32(EquipParamGemOffsetDict[AshOfWarID] + (int)EROffsets.EquipParamGem.canMountWep_Dagger) ?? 0; 
            set => EquipParamGem?.WriteUInt32(EquipParamGemOffsetDict[AshOfWarID] + (int)EROffsets.EquipParamGem.canMountWep_Dagger, value); 
        }
        public byte AshOfWarBitfield2
        {
            get => EquipParamGem?.ReadByte(EquipParamGemOffsetDict[AshOfWarID] + (int)EROffsets.EquipParamGem.canMountWep_ShieldSmall) ?? 0;
            set => EquipParamGem?.WriteByte(EquipParamGemOffsetDict[AshOfWarID] + (int)EROffsets.EquipParamGem.canMountWep_ShieldSmall, value);
        }
        public bool Dagger
        {
            get => (AshOfWarBitfield1 & 1) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 1;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~1);
            }
        }
        public bool SwordNormal
        {
            get => (AshOfWarBitfield1 & 2) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 2;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~2);
            }
        }
        public bool SwordLarge
        {
            get => (AshOfWarBitfield1 & 4) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 4;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~4);
            }
        }
        public bool SwordGigantic
        {
            get => (AshOfWarBitfield1 & 8) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 8;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~8);
            }
        }
        public bool SabreNormal
        {
            get => (AshOfWarBitfield1 & 16) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 16;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~16);
            }
        }
        public bool SabreLarge
        {
            get => (AshOfWarBitfield1 & 32) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 32;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~32);
            }
        }
        public bool katana
        {
            get => (AshOfWarBitfield1 & 64) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 64;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~64);
            }
        }
        public bool SwordDoulbeEdge
        {
            get => (AshOfWarBitfield1 & 128) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 128;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~128);
            }
        }
        public bool SwordPierce
        {
            get => (AshOfWarBitfield1 & 256) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 256;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~256);
            }
        }
        public bool RapierHeavy
        {
            get => (AshOfWarBitfield1 & 512) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 512;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~512);
            }
        }
        public bool AxeNormal
        {
            get => (AshOfWarBitfield1 & 1024) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 1024;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~1024);
            }
        }
        public bool AxeLarge
        {
            get => (AshOfWarBitfield1 & 2048) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 2048;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~2048);
            }
        }
        public bool HammerNormal
        {
            get => (AshOfWarBitfield1 & 4096) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 4096;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~4096);
            }
        }
        public bool HammerLarge
        {
            get => (AshOfWarBitfield1 & 8192) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 8192;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~8192);
            }
        }
        public bool Flail
        {
            get => (AshOfWarBitfield1 & 16384) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 16384;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~16384);
            }
        }
        public bool SpearNormal
        {
            get => (AshOfWarBitfield1 & 32768) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 32768;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~32768);
            }
        }
        public bool SpearLarge
        {
            get => (AshOfWarBitfield1 & 65536) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 65536;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~65536);
            }
        }
        public bool SpearHeavy
        {
            get => (AshOfWarBitfield1 & 131072) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 131072;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~131072);
            }
        }
        public bool SpearAxe
        {
            get => (AshOfWarBitfield1 & 262144) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 262144;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~262144);
            }
        }
        public bool Sickle
        {
            get => (AshOfWarBitfield1 & 524288) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 524288;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~524288);
            }
        }
        public bool Knuckle
        {
            get => (AshOfWarBitfield1 & 1048576) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 1048576;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~1048576);
            }
        }
        public bool Claw
        {
            get => (AshOfWarBitfield1 & 2097152) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 2097152;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~2097152);
            }
        }
        public bool Whip
        {
            get => (AshOfWarBitfield1 & 4194304) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 4194304;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~4194304);
            }
        }
        public bool Axhammerlarge
        {
            get => (AshOfWarBitfield1 & 8388608) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 8388608;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~8388608);
            }
        }
        public bool BowSmall
        {
            get => (AshOfWarBitfield1 & 16777216) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 16777216;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~16777216);
            }
        }
        public bool BowNormal
        {
            get => (AshOfWarBitfield1 & 33554432) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 33554432;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~33554432);
            }
        }
        public bool BowLarge
        {
            get => (AshOfWarBitfield1 & 67108864) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 67108864;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~67108864);
            }
        }
        public bool Clossbow
        {
            get => (AshOfWarBitfield1 & 134217728) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 134217728;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~134217728);
            }
        }
        public bool Ballista
        {
            get => (AshOfWarBitfield1 & 268435456) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 268435456;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~268435456);
            }
        }
        public bool Staff
        {
            get => (AshOfWarBitfield1 & 536870912) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 536870912;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~536870912);
            }
        }
        public bool Sorcery
        {
            get => (AshOfWarBitfield1 & 1073741824) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 1073741824;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~1073741824);
            }
        }
        public bool Talisman
        {
            get => (AshOfWarBitfield1 & 2147483648) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield1 |= 2147483648;
                else
                    AshOfWarBitfield1 = (uint)(AshOfWarBitfield1 & ~2147483648);
            }
        }
        public bool ShieldSmall
        {
            get => (AshOfWarBitfield2 & 1) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield2 |= 1;
                else
                    AshOfWarBitfield2 = (byte)(AshOfWarBitfield2 & ~1);
            }
        }
        public bool ShieldNormal
        {
            get => (AshOfWarBitfield2 & 2) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield2 |= 2;
                else
                    AshOfWarBitfield2 = (byte)(AshOfWarBitfield2 & ~2);
            }
        }
        public bool SheildLarge
        {
            get => (AshOfWarBitfield2 & 4) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield2 |= 4;
                else
                    AshOfWarBitfield2 = (byte)(AshOfWarBitfield2 & ~4);
            }
        }
        public bool Torch
        {
            get => (AshOfWarBitfield2 & 8) != 0;
            set
            {
                if (value)
                    AshOfWarBitfield2 |= 8;
                else
                    AshOfWarBitfield2 = (byte)(AshOfWarBitfield2 & ~8);
            }
        }
    }
}
