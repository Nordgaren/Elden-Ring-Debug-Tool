using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    internal class ERWeapon : ERItem
    {
        public enum Infusion
        {
            Standard = 000,
            Heavy = 100,
            Keen = 200,
            Quality = 300,
            Fire = 400,
            FlameArt = 500,
            Lightning = 600,
            Sacred = 700,
            Magic = 800,
            Cold = 900,
            Posion = 1000,
            Blood = 1100,
            Occult = 1200,
        };

        public enum WeaponType
        {
            Dagger = 1, //Dagger
            StraightSword = 3, //SwordNormal
            Greatsword = 5, //SwordLarge
            ColossalSword = 7, //SwordGigantic
            CurvedSword = 9, //SabreNormal
            CurvedGreatsword = 11, //SabreLarge
            Katana = 13, //katana
            Twinblade = 14, //SwordDoulbeEdge
            ThrustingSword = 15, //SwordPierce
            HeavyThrustingSword = 16, //RapierHeavy
            Axe = 17, //AxeNormal
            Greataxe = 19, //AxeLarge
            Hammer = 21, //HammerNormal
            GreatHammer = 23, //HammerLarge
            Flail = 24, //Flail
            Spear = 25, //SpearNormal
            SpearLarge = 00, //SpearLarge
            GreatSpear = 28, //SpearHeavy
            Halberd = 29, //SpearAxe
            Reaper = 31, //Sickle
            Fist = 35, //Knuckle
            Claws = 37, //Claw
            Whip = 39, //Whip
            ColossalWeapon = 41, //Axhammerlarge
            LightBow = 50, //BowSmall
            Bow = 51, //BowNormal
            Greatbow = 53, //BowLarge
            Crossbow = 55, //Clossbow
            Ballista = 56, //Ballista
            GlintstoneStaff = 57, //Staff
            Sorcery = 00, //Sorcery
            FingerSeal = 61, //Talisman
            SmallShield = 65, //ShieldSmall
            MediumShield = 67, //ShieldNormal
            Greatshield = 69, //SheildLarge
            Torch = 87 //Torch
        }

        public enum WeaponTypeUnused
        {
            Unarmed = 33,
            Arrow = 81,
            GreatArrow = 83,
            Bolt = 85,
            BallistaBolt = 86
        }
        public int RealID { get; set; }
        public bool Unique { get; set; }
        public int SwordArtId { get; set; }
        public bool Infisible { get; set; }
        public WeaponType Type { get; set; }
        public ERWeapon(string config, Category category) : base(config, category) 
        {
            RealID = Util.DeleteFromEnd(ID, 4);
        }

        public override void SetupItem(ERParam param)
        {
            var bitfield = param.Bytes[(int)EROffsets.EquipParamWeapon.DisableMultiDropShare];
            Infisible = (bitfield & (1 << 8)) == 1;
            if (Unique)
                Console.WriteLine();
            Unique = BitConverter.ToInt32(param.Bytes, param.OffsetDict[ID] + (int)EROffsets.EquipParamWeapon.MaterialSetID) == 2200;
        }
    }
}
