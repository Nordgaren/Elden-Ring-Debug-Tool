using Keystone;
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
        public event EventHandler<PHEventArgs> OnSetup;
        private void RaiseOnSetup()
        {
            OnSetup?.Invoke(this, new PHEventArgs(this));
        }

        private PHPointer CSSystemStep { get; set; }
        private PHPointer IsLoaded { get; set; }
        private PHPointer GameDataMan { get; set; }
        private PHPointer PlayerGameData { get; set; }
        private PHPointer SoloParamRepository { get; set; }
        private PHPointer CapParamCall { get; set; }
        public PHPointer EquipParamWeapon { get; set; }
        public PHPointer EquipParamGem { get; set; }
     
        public List<PHPointer> Params;
        //private PHPointer DurabilityAddr { get; set; }
        //private PHPointer DurabilitySpecialAddr { get; set; }
        public bool Loaded => PlayerGameData != null ?  PlayerGameData.Resolve() != IntPtr.Zero : false;
        private bool Setup = false;
        public ERHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector)
            : base(refreshInterval, minLifetime, processSelector)
        {
            OnHooked += ERHook_OnHooked;

            CSSystemStep = RegisterAbsoluteAOB(EROffsets.CSSystemStepAoB, EROffsets.CSSystemStepOffset);
            IsLoaded = CreateChildPointer(CSSystemStep, EROffsets.IsLoadedOffset1, EROffsets.IsLoadedOffset2, EROffsets.IsLoadedOffset3, EROffsets.IsLoadedOffset4);
            GameDataMan = RegisterRelativeAOB(EROffsets.GameDataManSetupAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);
            PlayerGameData = CreateChildPointer(GameDataMan, EROffsets.PlayerGameData);

            SoloParamRepository = RegisterRelativeAOB(EROffsets.SoloParamRepositorySetupAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);
            EquipParamWeapon = CreateChildPointer(SoloParamRepository, EROffsets.EquipParamWeaponOffset1, EROffsets.EquipParamWeaponOffset2, EROffsets.EquipParamWeaponOffset3);
            EquipParamGem = CreateChildPointer(SoloParamRepository, EROffsets.EquipParamGemOffset1, EROffsets.EquipParamGemOffset2, EROffsets.EquipParamGemOffset3);

            CapParamCall = RegisterAbsoluteAOB(EROffsets.CapParamCallAoB);
        }


        private void ERHook_OnHooked(object? sender, PHEventArgs e)
        {
            var isLoaded = IsLoaded.ReadByte(EROffsets.IsLoaded);
            while (isLoaded < 2)
            {
                isLoaded  = IsLoaded.ReadByte(EROffsets.IsLoaded);
            }

            Params = new List<PHPointer>();

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
            OnPropertyChanged(nameof(Setup));

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


        internal PHPointer GetParamPointer(int offset)
        {
            return CreateChildPointer(SoloParamRepository, new int[] { offset, 0x80, 0x80 });
        }
        internal void SaveParam(ERParam param)
        {
            var asmString = Util.GetEmbededResource("Assembly.SaveParams.asm");
            var asm = string.Format(asmString, SoloParamRepository.Resolve(), param.Offset, CapParamCall.Resolve());
            AsmExecute(asm);
        }

        private Engine Engine = new Engine(Architecture.X86, Mode.X64);
        //TKCode
        private void AsmExecute(string asm)
        {
            // Assemble once to determine size
            var bytes = Engine.Assemble(asm, 0);
            var error = Engine.GetLastKeystoneError();
            IntPtr insertPtr = Allocate((uint)bytes.Buffer.Length, Kernel32.PAGE_EXECUTE_READWRITE);
            // Then rebase and inject
            // Note: you can't use String.Format here because IntPtr is not IFormattable
            Kernel32.WriteBytes(Handle, insertPtr, bytes.Buffer);

            Debug.WriteLine("");
            foreach (var b in bytes.Buffer)
            {
                Debug.Write($"{b.ToString("X2")} ");
            }
            Debug.WriteLine("");

            Execute(insertPtr);
            Free(insertPtr);
        }

        public void RestoreParams()
        {
            if (!Setup)
                return;

            EquipParamWeapon.WriteBytes((int)EROffsets.Param.TotalParamLength, EquipParamWeaponBytes);
            EquipParamGem.WriteBytes((int)EROffsets.Param.TotalParamLength, EquipParamGemBytes);
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

    }
}
