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
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

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

        private PHPointer GameDataMan { get; set; }
        private PHPointer PlayerGameData { get; set; }
        private PHPointer PlayerInventory { get; set; }
        private PHPointer SoloParamRepository { get; set; }
        private PHPointer CapParamCall { get; set; }
        public PHPointer ItemGive { get; set; }
        public PHPointer MapItemMan { get; set; }
        public PHPointer EventFlagMan { get; set; }
        public PHPointer SetEventFlagFunction { get; set; }
        public PHPointer WorldChrMan { get; set; }
        public PHPointer PlayerIns { get; set; }
        public PHPointer DisableOpenMap { get; set; }
        public PHPointer CombatCloseMap { get; set; }
        public PHPointer WorldAreaWeather { get; set; }
        public static bool Reading { get; set; }
        public string ID => Process?.Id.ToString() ?? "Not Hooked";
        public List<PHPointer> Params;
        //private PHPointer DurabilityAddr { get; set; }
        //private PHPointer DurabilitySpecialAddr { get; set; }
        public bool Loaded => PlayerIns != null ? PlayerIns.Resolve() != IntPtr.Zero : false;
        public bool Setup = false;
        public bool Focused => Hooked && User32.GetForegroundProcessID() == Process.Id;

        public ERHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector)
            : base(refreshInterval, minLifetime, processSelector)
        {
            OnHooked += ERHook_OnHooked;

            GameDataMan = RegisterRelativeAOB(EROffsets.GameDataManAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);
            PlayerGameData = CreateChildPointer(GameDataMan, EROffsets.PlayerGameData);
            PlayerInventory = CreateChildPointer(PlayerGameData, EROffsets.EquipInventoryDataOffset, EROffsets.PlayerInventoryOffset);

            SoloParamRepository = RegisterRelativeAOB(EROffsets.SoloParamRepositoryAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);

            ItemGive = RegisterAbsoluteAOB(EROffsets.ItemGiveAoB);
            MapItemMan = RegisterRelativeAOB(EROffsets.MapItemManAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize);
            EventFlagMan = RegisterRelativeAOB(EROffsets.EventFlagManAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);
            SetEventFlagFunction = RegisterAbsoluteAOB(EROffsets.EventCallAoB);

            CapParamCall = RegisterAbsoluteAOB(EROffsets.CapParamCallAoB);

            WorldChrMan = RegisterRelativeAOB(EROffsets.WorldChrManAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);
            PlayerIns = CreateChildPointer(WorldChrMan, EROffsets.PlayerInsOffset);

            DisableOpenMap = RegisterAbsoluteAOB(EROffsets.DisableOpenMapAoB);
            CombatCloseMap = RegisterAbsoluteAOB(EROffsets.CombatCloseMapAoB);
            WorldAreaWeather = RegisterRelativeAOB(EROffsets.WorldAreaWeatherAoB, EROffsets.RelativePtrAddressOffset, EROffsets.RelativePtrInstructionSize, 0x0);

            BuildItemEventDictionary();
        }


        private void ERHook_OnHooked(object? sender, PHEventArgs e)
        {
            //var gameDataMan = GameDataMan.Resolve();
            var paramss = SoloParamRepository.Resolve();
            //var itemGive = ItemGive.Resolve();
            //var mapItemMan = MapItemMan.Resolve();
            //var eventFlagMan = EventFlagMan.Resolve();
            //var setEventFlagFunction = SetEventFlagFunction.Resolve(); 
            //var capParamCall = CapParamCall.Resolve();
            //var worldChrMan = WorldChrMan.Resolve();

            //var disableOpenMap = DisableOpenMap.Resolve();
            //var combatCloseMap = CombatCloseMap.Resolve();


            RaiseOnSetup();
            ReadParams();
            Setup = true;
        }
        public void Update()
        {
            OnPropertyChanged(nameof(Setup));
            OnPropertyChanged(nameof(ID));

            if (!Setup)
                return;

            OnPropertyChanged(nameof(Loaded));
            OnPropertyChanged(nameof(InventoryCount));
            OnPropertyChanged(nameof(TargetEnemyHandle));
            OnPropertyChanged(nameof(TargetHp));
            OnPropertyChanged(nameof(TargetHpMax));
            OnPropertyChanged(nameof(TargetFp));
            OnPropertyChanged(nameof(TargetFpMax));
            OnPropertyChanged(nameof(TargetStam));
            OnPropertyChanged(nameof(TargetStamMax));
            OnPropertyChanged(nameof(TargetPoison));
            OnPropertyChanged(nameof(TargetPoisonMax));
            OnPropertyChanged(nameof(TargetRot));
            OnPropertyChanged(nameof(TargetRotMax));
            OnPropertyChanged(nameof(TargetBleed));
            OnPropertyChanged(nameof(TargetBleedMax));
            OnPropertyChanged(nameof(TargetBlight));
            OnPropertyChanged(nameof(TargetBlightMax));
            OnPropertyChanged(nameof(TargetFrost));
            OnPropertyChanged(nameof(TargetFrostMax));
            OnPropertyChanged(nameof(TargetSleep));
            OnPropertyChanged(nameof(TargetSleepMax));
            OnPropertyChanged(nameof(TargetMadness));
            OnPropertyChanged(nameof(TargetMadnessMax));
            OnPropertyChanged(nameof(TargetStagger));
            OnPropertyChanged(nameof(TargetStaggerMax));
            OnPropertyChanged(nameof(TargetResetTime));
        }

        public ERParam EquipParamAccessory;
        public ERParam EquipParamGem;
        public ERParam EquipParamGoods;
        public ERParam EquipParamProtector;
        public ERParam EquipParamWeapon;
        public ERParam MagicParam;

        private Engine Engine = new Engine(Architecture.X86, Mode.X64);
        //TKCode
        private void AsmExecute(string asm)
        {
            //Assemble once to get the size
            var bytes = Engine.Assemble(asm, (ulong)Process.MainModule.BaseAddress);
            //DebugPrintArray(bytes.Buffer);
            var error = Engine.GetLastKeystoneError();
            if (error != KeystoneError.KS_ERR_OK)
                throw new Exception("Something went wrong during assembly. Code could not be assembled.");

            IntPtr insertPtr = GetPrefferedIntPtr(bytes.Buffer.Length, Kernel32.PAGE_EXECUTE_READWRITE);

            //Reassemble with the location of the isertPtr to support relative instructions
            bytes = Engine.Assemble(asm, (ulong)insertPtr);
            error = Engine.GetLastKeystoneError();

            Kernel32.WriteBytes(Handle, insertPtr, bytes.Buffer);
#if DEBUG
            DebugPrintArray(bytes.Buffer);
#endif

            Execute(insertPtr);
            Free(insertPtr);
        }

#if DEBUG
        private static void DebugPrintArray(byte[] bytes)
        {
            Debug.WriteLine("");
            foreach (var b in bytes)
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

        public void AddParam(List<ERParam> paramList, string paramPath, string path, string[] pointers)
        {
            foreach (var entry in pointers)
            {
                if (!Util.IsValidTxtResource(entry))
                    continue;

                var info = entry.TrimComment().Split(':');
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
            var asmString = Util.GetEmbededResource("Assembly.SaveParams.asm");
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


        #endregion

        public void SetEventFlag(int flag)
        {
            var idPointer = GetPrefferedIntPtr(sizeof(int));
            Kernel32.WriteInt32(Handle, idPointer, flag);

            var asmString = Util.GetEmbededResource("Assembly.SetEventFlag.asm");
            var asm = string.Format(asmString, EventFlagMan.Resolve(), idPointer.ToString("X2"), SetEventFlagFunction.Resolve());
            AsmExecute(asm);
            Free(idPointer);
        }

        #region Inventory

        private static Regex ItemEventEntryRx = new Regex(@"^(?<event>\S+) (?<item>\S+)$", RegexOptions.CultureInvariant);

        private static Dictionary<int, int> ItemEventDictionary;

        private void BuildItemEventDictionary()
        {
            ItemEventDictionary = new Dictionary<int, int>();
            var goodsEvents = Util.GetTxtResource("Resources/Events/GoodsEvents.txt");
            foreach (var line in goodsEvents.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Util.IsValidTxtResource(line))
                {
                    Match itemEntry = ItemEventEntryRx.Match(line.TrimComment());
                    var eventID = Convert.ToInt32(itemEntry.Groups["event"].Value);
                    var itemID = Convert.ToInt32(itemEntry.Groups["item"].Value);
                    ItemEventDictionary.Add(itemID + (int)Category.Goods, eventID);
                }
            }
        }
        private void ReadParams()
        {
            foreach (var category in ERItemCategory.All)
            {
                foreach (var item in category.Items)
                {
                    SetupItem(item);
                    var fullID = item.ID + (int)Category.Goods;
                    item.EventID = ItemEventDictionary.ContainsKey(fullID) ? ItemEventDictionary[fullID] : -1;
                }
            }

            foreach (var category in ERItemCategory.All)
            {
                if (category.Category == Category.Weapons)
                    foreach (ERWeapon weapon in category.Items)
                    {
                        var gem = ERGem.All.FirstOrDefault(gem => gem.SwordArtID == weapon.SwordArtId);
                        if (gem != null)
                            weapon.DefaultGem = gem;
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

        internal void GetItem(int id, int quantity, int infusion, int upgrade, int gem)
        {
            var itemInfobytes = new byte[0x34];
            var itemInfo = GetPrefferedIntPtr(0x34);

            var bytes = BitConverter.GetBytes(0x1);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)EROffsets.ItemGiveStruct.Count, bytes.Length);

            bytes = BitConverter.GetBytes(id + infusion + upgrade);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)EROffsets.ItemGiveStruct.ID, bytes.Length);

            bytes = BitConverter.GetBytes(quantity);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)EROffsets.ItemGiveStruct.Quantity, bytes.Length);

            bytes = BitConverter.GetBytes(gem);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)EROffsets.ItemGiveStruct.Gem, bytes.Length);

            Kernel32.WriteBytes(Handle, itemInfo, itemInfobytes);

            var asmString = Util.GetEmbededResource("Assembly.ItemGive.asm");
            var asm = string.Format(asmString, itemInfo.ToString("X2"), MapItemMan.Resolve(), ItemGive.Resolve() + EROffsets.ItemGiveOffset);
            AsmExecute(asm);
            Free(itemInfo);
        }

        List<ERInventoryEntry> Inventory;
        public int InventoryCount => PlayerGameData.ReadInt32((int)EROffsets.PlayerGameDataStruct.InventoryCount);
        public int LastInventoryCount { get; set; }

        internal IEnumerable GetInventory()
        {
            if (InventoryCount != LastInventoryCount)
                GetInventoryList();

            return Inventory;
        }
        private void GetInventoryList()
        {
            Inventory = new List<ERInventoryEntry>();
            LastInventoryCount = InventoryCount;

            var bytes = PlayerInventory.ReadBytes(0x0, (uint)InventoryCount * EROffsets.PlayerInventoryEntrySize);

            for (int i = 0; i < InventoryCount; i++)
            {
                var entry = new byte[EROffsets.PlayerInventoryEntrySize];
                Array.Copy(bytes, i * EROffsets.PlayerInventoryEntrySize, entry, 0, entry.Length);
                Inventory.Add(new ERInventoryEntry(entry, this));
            }
        }

        public void ResetInventory()
        {
            Inventory = new List<ERInventoryEntry>();
            LastInventoryCount = 0;
        }
        #endregion

        #region Target  

        private int CurrentTargetHandle => PlayerIns?.ReadInt32((int)EROffsets.PlayerIns.TargetHandle) ?? 0;
        private int CurrentTargetArea => PlayerIns?.ReadInt32((int)EROffsets.PlayerIns.TargetArea) ?? 0;
        private PHPointer _targetEnemyIns { get; set; }
        private PHPointer TargetEnemyIns
        {
            get => _targetEnemyIns;
            set
            {
                _targetEnemyIns = value;
                TargetEnemyModuleBase = CreateChildPointer(_targetEnemyIns, (int)EROffsets.EnemyIns.ModuleBase);
                TargetEnemyData = CreateChildPointer(TargetEnemyModuleBase, (int)EROffsets.ModuleBase.EnemyData);
                TargetEnemyResistance = CreateChildPointer(TargetEnemyModuleBase, (int)EROffsets.ModuleBase.ResistenceData);
                TargetEnemyStagger = CreateChildPointer(TargetEnemyModuleBase, (int)EROffsets.ModuleBase.StaggerData);
            }
        }
        public int TargetEnemyHandle => PlayerIns?.ReadInt32((int)EROffsets.EnemyIns.EnemyHandle) ?? 0;
        public int TargetEnemyArea => PlayerIns?.ReadInt32((int)EROffsets.EnemyIns.EnemyArea) ?? 0;
        private PHPointer TargetEnemyModuleBase;
        private PHPointer TargetEnemyData;
        private PHPointer TargetEnemyResistance;
        private PHPointer TargetEnemyStagger;

        private int TargetHandle => _targetEnemyIns?.ReadInt32((int)EROffsets.EnemyIns.EnemyHandle) ?? 0;
        //public string Model => TargetEnemy?.ReadString((int)EROffsets.EnemyData.Model, );
        //public string Name => TargetEnemy?.ReadString((int)EROffsets.EnemyData.Name, );
        public int TargetHp
        {
            get => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.Hp) ?? 0;
            set => _ = value; 
        }
        public int TargetHpMax
        {
            get => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.HpMax) ?? 0;
            set => _ = value;

        }
        public int TargetFp
        {
            get => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.Fp) ?? 0;
            set => _ = value;

        }
        public int TargetFpMax
        {
            get => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.FpMax) ?? 0;
            set => _ = value;

        }
        public int TargetStam
        {
            get => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.Stam) ?? 0;
            set => _ = value;

        }
        public int TargetStamMax
        {
            get => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.StamMax) ?? 0;
            set => _ = value;

        }
        public int TargetPoison
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Poison) ?? 0;
            set => _ = value;

        }
        public int TargetPoisonMax
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.PoisonMax) ?? 0;
            set => _ = value;

        }
        public int TargetRot
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Rot) ?? 0;
            set => _ = value;

        }
        public int TargetRotMax
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.RotMax) ?? 0;
            set => _ = value;

        }
        public int TargetBleed
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Bleed) ?? 0;
            set => _ = value;

        }
        public int TargetBleedMax
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.BleedMax) ?? 0;
            set => _ = value;

        }
        public int TargetFrost
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Frost) ?? 0;
            set => _ = value;

        }
        public int TargetFrostMax
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.FrostMax) ?? 0;
            set => _ = value;

        }
        public int TargetBlight
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Blight) ?? 0;
            set => _ = value;

        }
        public int TargetBlightMax
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.BlightMax) ?? 0;
            set => _ = value;

        }
        public int TargetSleep
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Sleep) ?? 0;
            set => _ = value;

        }
        public int TargetSleepMax
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.SleepMax) ?? 0;
            set => _ = value;

        }
        public int TargetMadness
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Madness) ?? 0;
            set => _ = value;

        }
        public int TargetMadnessMax
        {
            get => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.MadnessMax) ?? 0;
            set => _ = value;

        }
        public float TargetStagger
        {
            get => TargetEnemyStagger?.ReadSingle((int)EROffsets.StaggerData.Stagger) ?? 0;
            set => _ = value;

        }
        public float TargetStaggerMax
        {
            get => TargetEnemyStagger?.ReadSingle((int)EROffsets.StaggerData.StaggerMax) ?? 0;
            set => _ = value;

        }
        public float TargetResetTime
        {
            get => TargetEnemyStagger?.ReadSingle((int)EROffsets.StaggerData.ResetTime) ?? 0;
            set => _ = value;
        }

        public void UpdateLastEnemy()
        {
            if (CurrentTargetHandle == -1 || CurrentTargetHandle == TargetHandle)
                return;

            GetTarget();
        }

        public void GetTarget()
        {
            TargetEnemyIns = null;
            var count = WorldChrMan.ReadInt32((int)EROffsets.WorldChrMan.NumWorldBlockChr);
            var worldBlockChr = CreateBasePointer(WorldChrMan.Resolve() + (int)EROffsets.WorldChrMan.WorldBlockChr);
            var targetHandle = CurrentTargetHandle; //Only read from memory once
            var targetArea = CurrentTargetArea;

            for (int i = 0; i <= count; i++)
            {
                var numChrs = worldBlockChr.ReadInt32((int)EROffsets.WorldBlockChr.NumChr + (i * 0x160));
                var chrSet = CreateChildPointer(worldBlockChr, (int)EROffsets.WorldBlockChr.ChrSet + (i * 0x160));

                for (int j = 0; j <= numChrs; j++)
                {
                    var enemyIns = CreateBasePointer(chrSet.Resolve() + (j * (int)EROffsets.ChrSet.EnemyIns));
                    var enemyHandle = enemyIns.ReadInt32((int)EROffsets.EnemyIns.EnemyHandle);
                    var enemyArea = enemyIns.ReadInt32((int)EROffsets.EnemyIns.EnemyArea);

                    if (targetHandle == enemyHandle && targetArea == enemyArea)
                        TargetEnemyIns = enemyIns;

                    if (TargetEnemyIns != null)
                        return;
                }

            }

            var success = TryGetEnemy(targetHandle, targetArea, (int)EROffsets.WorldChrMan.ChrSet1);

            if (success)
                return;

            success = TryGetEnemy(targetHandle, targetArea, (int)EROffsets.WorldChrMan.ChrSet2);

        }

        public bool TryGetEnemy(int targetHandle, int targetArea, int offset)
        {
            var chrSet1 = CreateChildPointer(WorldChrMan, offset);
            var numEntries1 = chrSet1.ReadInt32((int)EROffsets.ChrSet.NumEntries);

            for (int i = 0; i <= numEntries1; i++)
            {
                var enemyHandle = chrSet1.ReadInt32(0x78 + (i * 0x10));
                var enemyArea = chrSet1.ReadInt32(0x78 + 4 + (i * 0x10));
                if (targetHandle == enemyHandle && targetArea == enemyArea)
                    TargetEnemyIns = CreateChildPointer(chrSet1, 0x78 + 8 + (i * 0x10));

                if (TargetEnemyIns != null)
                    return true;
            }

            return false;
        }
        #endregion

        public int Level => PlayerGameData.ReadInt32((int)EROffsets.Player.Level);
        public string LevelString => PlayerGameData?.ReadInt32((int)EROffsets.Player.Level).ToString() ?? "";

        #region Cheats

        byte[] OriginalCombatCloseMap;
        private bool _enableMapCombat { get; set; }

        public bool EnableMapCombat
        {
            get => _enableMapCombat;
            set
            {
                _enableMapCombat = value;
                if (value)
                    EnableMapInCombat();
                else
                    DisableMapInCombat();
            }
        }

        private void EnableMapInCombat()
        {
            OriginalCombatCloseMap = CombatCloseMap.ReadBytes(0x0, 0x5);
            var assembly = new byte[] { 0x48, 0x31, 0xC0, 0x90, 0x90 };

            DisableOpenMap.WriteByte(0x0, 0xEB); //Write Jump
            CombatCloseMap.WriteBytes(0x0, assembly);
        }

        private void DisableMapInCombat()
        {
            DisableOpenMap.WriteByte(0x0, 0x74); //Write Jump Equals
            CombatCloseMap.WriteBytes(0x0, OriginalCombatCloseMap); //Place original bytes back for combat close map
        }
        private short ForceWeatherParamID 
        {
            set => WorldAreaWeather?.WriteInt16((int)EROffsets.WorldAreaWeather.ForceWeatherParamID, value); 
        }
        public enum WeatherTypes
        {
            Unk0 = 0,
            Unk1 = 1,
            Unk10 = 10,
            Unk11 = 11,
            Unk20 = 20,
            Unk21 = 21,
            Unk30 = 30,
            Unk31 = 31,
            Unk40 = 40,
            Unk41 = 41,
            Unk50 = 50,
            Unk51 = 51,
            Unk52 = 52,
            Unk60 = 60,
            Unk81 = 81,
            Unk82 = 82,
            Unk83 = 83,
            Clear = 99
        }
        private WeatherTypes _selectedWeather;
        public WeatherTypes SelectedWeather {
            get => _selectedWeather;
            set 
            { 
                _selectedWeather = value;
                if (_forceWeather)
                    ForceWeatherParamID = (short)_selectedWeather;
            }
        }
        private bool _forceWeather;
        public bool ForceWeather
        {
            get { return _forceWeather; }
            set 
            {
                ForceWeatherParamID = (short)_selectedWeather;
                _forceWeather = value; 
            }
        }

        public void ForceSetWeather()
        {
            ForceWeatherParamID = (short)_selectedWeather;
        }

        #endregion

        #region ChrAsm
        public byte ArmStyle
        {
            get => PlayerGameData.ReadByte((int)EROffsets.ChrIns.ArmStyle);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteByte((int)EROffsets.ChrIns.ArmStyle, value);
            }
        }
        public int CurrWepSlotOffsetLeft
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.CurrWepSlotOffsetLeft);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.CurrWepSlotOffsetLeft, value);
            }
        }
        public int CurrWepSlotOffsetRight
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.CurrWepSlotOffsetRight);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.CurrWepSlotOffsetRight, value);
            }
        }
        public int RHandWeapon1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.RHandWeapon1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.RHandWeapon1, value);
            }
        }
        public int RHandWeapon2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.RHandWeapon2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.RHandWeapon2, value);
            }
        }
        public int RHandWeapon3
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.RHandWeapon3);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.RHandWeapon3, value);
            }
        }
        public int LHandWeapon1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.LHandWeapon1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.LHandWeapon1, value);
            }
        }
        public int LHandWeapon2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.LHandWeapon2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.LHandWeapon2, value);
            }
        }
        public int LHandWeapon3
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.LHandWeapon3);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.LHandWeapon3, value);
            }
        }
        public int Arrow1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.Arrow1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.Arrow1, value);
            }
        }
        public int Arrow2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.Arrow2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.Arrow2, value);
            }
        }
        public int Bolt1
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.Bolt1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.Bolt1, value);
            }
        }
        public int Bolt2
        {
            get => PlayerGameData.ReadInt32((int)EROffsets.ChrIns.Bolt2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)EROffsets.ChrIns.Bolt2, value);
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
