using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infusion = Elden_Ring_Debug_Tool.ERWeapon.Infusion;
using WeaponType = Elden_Ring_Debug_Tool.ERWeapon.WeaponType;

namespace Elden_Ring_Debug_Tool
{
    class ERGem : ERItem
    {
        public static List<ERGem> All = new List<ERGem>();

        public long CanMountBitfield;
        public int SwordArtID;
        public short WeaponAttr;
        public List<Infusion> Infusions;
        public List<WeaponType> WeaponTypes = new List<WeaponType>();
        public static ERGem Default = new ERGem("-1 None", Category.Gem, false);

        private void GetInfusions()
        {
            Infusions = new List<Infusion>() { Infusion.Standard };

            if (WeaponAttr == 0)
                return;

            for (int i = 1; i < AllInfusions.Count; i++)
            {
                if ((WeaponAttr & (1 << i)) != 0)
                    Infusions.Add(AllInfusions[i]);
            }
        }

        private void GetWeapons()
        {
            WeaponTypes = new List<WeaponType>();

            if (CanMountBitfield == 0)
                return;

            for (int i = 0; i < Weapons.Count; i++)
            {
                if ((CanMountBitfield & (1L << i)) != 0)
                    WeaponTypes.Add(Weapons[i]);
            }
        }

        public override void SetupItem(ERParam param)
        {
            MaxQuantity = 1;
            var bitfield = param.Bytes[param.OffsetDict[ID] + (int)EROffsets.EquipParamGem.IsDiscard];
            IsDrop = (bitfield & (1 << 1)) != 0;
            IsMultiplayerShare = (bitfield & (1 << 3)) == 0;

            SwordArtID = BitConverter.ToInt32(param.Bytes, param.OffsetDict[ID] + (int)EROffsets.EquipParamGem.SwordArtsParamId);
            CanMountBitfield = BitConverter.ToInt64(param.Bytes, param.OffsetDict[ID] + (int)EROffsets.EquipParamGem.CanMountWep_Dagger);
            WeaponAttr = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)EROffsets.EquipParamGem.ConfigurableWepAttr);
            GetWeapons();
            GetInfusions();
        }

        public ERGem(string config, Category category, bool showIDs) : base(config, category, showIDs)
        {
            All.Add(this);
        }

        static ERGem()
        {
            Default.Infusions = new List<Infusion>() { Infusion.Standard };
        }

        public static List<WeaponType> Weapons = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();

        public static List<Infusion> AllInfusions = Enum.GetValues(typeof(Infusion)).Cast<Infusion>().ToList();

    }
}
