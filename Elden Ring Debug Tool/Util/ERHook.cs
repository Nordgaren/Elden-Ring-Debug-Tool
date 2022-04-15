using Keystone;
using PropertyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Infusion = Elden_Ring_Debug_Tool.ERWeapon.Infusion;
using Category = Elden_Ring_Debug_Tool.ERItem.Category;
using WeaponType = Elden_Ring_Debug_Tool.ERWeapon.WeaponType;
using static SoulsFormats.PARAMDEF;


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
        public PHPointer ItemGive { get; set; }
        public PHPointer MapItemMan { get; set; }
        public static bool Reading { get; set; }
        public string ID => Process?.Id.ToString() ?? "Not Hooked";
        public List<PHPointer> Params;
        //private PHPointer DurabilityAddr { get; set; }
        //private PHPointer DurabilitySpecialAddr { get; set; }
        public bool Loaded => PlayerGameData != null ?  PlayerGameData.Resolve() != IntPtr.Zero : false;
        public bool Setup = false;
        public ERHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector)
            : base(refreshInterval, minLifetime, processSelector)
        {
            OnHooked += ERHook_OnHooked;

            CSSystemStep = RegisterAbsoluteAOB(EROffsets.CSSystemStepAoB, EROffsets.CSSystemStepOffset);
            IsLoaded = CreateChildPointer(CSSystemStep, EROffsets.IsLoadedOffset1, EROffsets.IsLoadedOffset2, EROffsets.IsLoadedOffset3, EROffsets.IsLoadedOffset4);
            GameDataMan = RegisterRelativeAOB(EROffsets.GameDataManAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);
            PlayerGameData = CreateChildPointer(GameDataMan, EROffsets.PlayerGameData);

            SoloParamRepository = RegisterRelativeAOB(EROffsets.SoloParamRepositoryAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);

            ItemGive = RegisterAbsoluteAOB(EROffsets.ItemGiveAoB);
            MapItemMan = RegisterRelativeAOB(EROffsets.MapItemManAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize);


            CapParamCall = RegisterAbsoluteAOB(EROffsets.CapParamCallAoB);
        }


        private void ERHook_OnHooked(object? sender, PHEventArgs e)
        {
            RaiseOnSetup();
            ReadParams();
            Setup = true;
        }
        public void Update()
        {
            if (!Setup)
                return;

            OnPropertyChanged(nameof(Loaded));
            OnPropertyChanged(nameof(Setup));

        }
        private void ReadParams()
        {
            foreach (var category in ERItemCategory.All)
            {
                foreach (var item in category.Items)
                {
                    SetupItem(item);
                }
            }
        }

        private void SetupItem(ERItem item)
        {
            switch (item.ItemCategory)
            {
                case Category.Weapons:
                    item.SetupItem(EquipParamWeapon);
                    break;
                case Category.Protector:
                    item.SetupItem(EquipParamProtector);
                    break;
                case Category.Accessory:
                    item.SetupItem(EquipParamAccessory);
                    break;
                case Category.Goods:
                    item.SetupItem(EquipParamGoods);
                    break;
                case Category.Gem:
                    item.SetupItem(EquipParamGem);
                    break;
                default:
                    break;
            }
        }

        private ERParam EquipParamAccessory;
        private ERParam EquipParamGem;
        private ERParam EquipParamGoods;
        private ERParam EquipParamProtector;
        private ERParam EquipParamWeapon;
        private ERParam MagicParam;


        private Engine Engine = new Engine(Architecture.X86, Mode.X64);
        //TKCode
        private void AsmExecute(string asm)
        {
            //Assemble once to get the size
            var bytes = Engine.Assemble(asm, (ulong)Process.MainModule.BaseAddress);
            var error = Engine.GetLastKeystoneError();
            IntPtr insertPtr = GetPrefferedIntPtr(bytes.Buffer.Length, Kernel32.PAGE_EXECUTE_READWRITE);

            //Reassemble with the location if the intPtr to support things like Call and Jmp
            bytes = Engine.Assemble(asm, (ulong)insertPtr);
            error = Engine.GetLastKeystoneError();

            Kernel32.WriteBytes(Handle, insertPtr, bytes.Buffer);


#if DEBUG
            //DebugPrintArray(bytes);
#endif

            Execute(insertPtr);
            Free(insertPtr);
        }

#if DEBUG
        private static void DebugPrintArray(EncodedData bytes)
        {
            Debug.WriteLine("");
            foreach (var b in bytes.Buffer)
            {
                Debug.Write($"{b.ToString("X2")} ");
            }
            Debug.WriteLine("");
        }
#endif

        #region Params

        public List<ERParam> GetParams()
        {
            var paramList = new List<ERParam>();
            var paramPath = $"{Util.ExeDir}/Resources/Params/";

            var pointerPath = $"{paramPath}/Pointers/";
            var paramPointers = Directory.GetFiles(pointerPath, "*.txt");
            foreach (var path in paramPointers)
            {
                var pointers = File.ReadAllLines(path);
                AddParam(paramList, paramPath, path, pointers);
            }

            return paramList;
        }

        public void AddParam(List<ERParam> paramList,string paramPath, string path, string[] pointers)
        {
            foreach (var entry in pointers)
            {
                if (!Util.IsValidTxtResource(entry))
                    continue;

                var info = entry.Split(':');
                var name = info[1];
                var defName = info.Length > 2 ? info[2] : name;

                var defPath = $"{paramPath}/Defs/{defName}.xml";
                if (!File.Exists(defPath))
                    throw new Exception($"The PARAMDEF {defName} does not exist for {entry}. If the PARAMDEF is named differently than the param name, add another \":\" and append the PARAMDEF name" +
                        $"Example: 3130:WwiseValueToStrParam_BgmBossChrIdConv:WwiseValueToStrConvertParamFormat");

                var offset = int.Parse(info[0], System.Globalization.NumberStyles.HexNumber);

                var pointer = GetParamPointer(offset);

                var paramDef = XmlDeserialize(defPath);

                var param = new ERParam(pointer, offset, paramDef, name);

                SetParamPtrs(param);

                paramList.Add(param);
            }
            paramList.Sort();
        }

        private void SetParamPtrs(ERParam param)
        {
            switch (param.Name)
            {
                case "EquipParamAccessory":
                    EquipParamAccessory = param;
                    break;
                case "EquipParamGem":
                    EquipParamGem = param;
                    break;
                case "EquipParamGoods":
                    EquipParamGoods = param;
                    break;
                case "EquipParamProtector":
                    EquipParamProtector = param;
                    break;
                case "EquipParamWeapon":
                    EquipParamWeapon = param;
                    break;
                case "Magic":
                    MagicParam = param;
                    break;
                default:
                    break;
            }
        }

        internal PHPointer GetParamPointer(int offset)
        {
            return CreateChildPointer(SoloParamRepository, new int[] { offset, 0x80, 0x80 });
        }
        internal void SaveParam(ERParam param)
        {
            var asmString = Util.GetEmbededResource("Resources.Assembly.SaveParams.asm");
            var asm = string.Format(asmString, SoloParamRepository.Resolve(), param.Offset, CapParamCall.Resolve());
            AsmExecute(asm);
        }
        public void RestoreParams()
        {
            if (!Setup)
                return;

            EquipParamWeapon.RestoreParam();
            EquipParamGem.RestoreParam();
        }

        internal void GetItem(int id, int quantity, int infusion, int upgrade, int gem)
        {
            var itemInfobytes = new byte[0x34];
            var itemInfo = GetPrefferedIntPtr(0x34);

            var bytes = BitConverter.GetBytes(0x1);
            Array.Copy(bytes, 0x0,itemInfobytes, (int)EROffsets.ItemGiveStruct.Count, bytes.Length);

            bytes = BitConverter.GetBytes(id + infusion + upgrade);
            Array.Copy(bytes, 0x0,itemInfobytes, (int)EROffsets.ItemGiveStruct.ID, bytes.Length);

            bytes = BitConverter.GetBytes(quantity);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)EROffsets.ItemGiveStruct.Quantity, bytes.Length);

            bytes = BitConverter.GetBytes(gem);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)EROffsets.ItemGiveStruct.Gem, bytes.Length);

            Kernel32.WriteBytes(Handle, itemInfo, itemInfobytes);

            var asmString = Util.GetEmbededResource("Resources.Assembly.ItemGive.asm");
            var asm = string.Format(asmString, itemInfo.ToString("X2"), MapItemMan.Resolve(), ItemGive.Resolve() + EROffsets.ItemGiveOffset);
            AsmExecute(asm);
            Free(itemInfo);
        }

        #endregion

        #region Inventory


        #endregion


        public int Level => PlayerGameData.ReadInt32((int)EROffsets.Player.Level);
        public string LevelString => PlayerGameData?.ReadInt32((int)EROffsets.Player.Level).ToString() ?? "";



        #region ChrAsm
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
        #endregion

        private int OGRHandWeapon1 { get; set; }
        private PHPointer OGRHandWeapon1Param
        {
            get => CreateBasePointer(EquipParamWeapon.Pointer.Resolve() + EquipParamWeapon.OffsetDict[OGRHandWeapon1]);
        }
        private int OGRHandWeapon1SwordArtID
        {
            get => OGRHandWeapon1Param?.ReadInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId) ?? 0;
            set => OGRHandWeapon1Param?.WriteInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId, value);
        }
        private int OGLHandWeapon1 { get; set; }
        private PHPointer OGLHandWeapon1Param
        {
            get => CreateBasePointer(EquipParamWeapon.Pointer.Resolve() + EquipParamWeapon.OffsetDict[OGLHandWeapon1]);
        }
        private int OGLHandWeapon1SwordArtID
        {
            get => OGLHandWeapon1Param?.ReadInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId) ?? 0;
            set => OGLHandWeapon1Param?.WriteInt32((int)EROffsets.EquipParamWeapon.SwordArtsParamId, value);
        }

    }
}
