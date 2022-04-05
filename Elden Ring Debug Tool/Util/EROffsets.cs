﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    internal class EROffsets
    {
        public const int BasePtrOffset1 = 0x3;
        public const int BasePtrOffset2 = 0x7;

        public const string GameDataManSetupAoB = "48 8B 05 ? ? ? ? 48 85 C0 74 05 48 8B 40 58 C3 C3";

        public const int PlayerGameData = 0x8;
        public enum Weapons
        {
            ArmStyle = 0x328,
            CurrWepSlotOffsetLeft = 0x32C,
            CurrWepSlotOffsetRight = 0x330,
            LHandWeapon1 = 0x39C,
            LHandWeapon2 = 0x3A4,
            LHandWeapon3 = 0x3AC,
            RHandWeapon1 = 0x3A0,
            RHandWeapon2 = 0x3A8,
            RHandWeapon3 = 0x3B0,
            Arrow1 = 0x3B4,
            Bolt1 = 0x3B8,
            Arrow2 = 0x3BC,
            Bolt2 = 0x3C0
        }

        public enum Player
        {
            Level = 0x68
        }

        public const string SoloParamRepositorySetupAoB = "48 8B 0D ? ? ? ? 48 85 C9 0F 84 ? ? ? ? 45 33 C0 BA 8E 00 00 00";

        public enum Param
        {
            TotalParamLength = 0x0,
            TableLength = 0x30,
        }

        public const int EquipParamWeaponOffset1 = 0x88;
        public const int EquipParamWeaponOffset2 = 0x80;
        public const int EquipParamWeaponOffset3 = 0x80;

        public enum EquipParamWeapon
        {
            MaterialSetID = 0x5C,
            ReinforceTypeID = 0xDA,
            SwordArtsParamId = 0x198,
            WepType = 0x1A6
        }

        public const int EquipParamGemOffset1 = 0x2BD8;
        public const int EquipParamGemOffset2 = 0x80;
        public const int EquipParamGemOffset3 = 0x80;

        public enum EquipParamGem
        {
            canMountWep_Dagger = 0x38,
            canMountWep_SwordPierce = 0x39,
            canMountWep_SpearLarge = 0x3A,
            canMountWep_BowSmall = 0x3B,
            canMountWep_ShieldSmall = 0x3C
        }
    }
}
