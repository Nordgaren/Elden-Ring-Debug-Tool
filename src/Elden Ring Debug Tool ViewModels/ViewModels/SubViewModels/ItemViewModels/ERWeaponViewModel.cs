using Erd_Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erd_Tools.ERWeapon;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERWeaponViewModel : ERItemViewModel
    {
        protected ERWeapon _weapon => _item as ERWeapon;
        public int RealID => _weapon.RealID;
        public bool Unique => _weapon.Unique;
        public int SwordArtId => _weapon.SwordArtId;
        public bool Infusible => _weapon.Infisible;
        public int MaxUpgrade => _weapon.MaxUpgrade;
        public WeaponType Type => _weapon.Type;
        public AmmoType TypeAmmo => _weapon.TypeAmmo;
        //public ERGemViewModel DefaultGem { get; set; }
        public ERWeaponViewModel(ERWeapon weapon) : base(weapon)
        {
        }

        public override void SetupItem()
        {

        }
 
    }
}
