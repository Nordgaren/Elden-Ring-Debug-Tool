using Erd_Tools;
using Erd_Tools.Models;
using static Erd_Tools.Models.Weapon;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class WeaponViewModel : ItemViewModel
    {
        protected Weapon _weapon => _item as Weapon;
        public int RealID => _weapon.RealID;
        public bool Unique => _weapon.Unique;
        public int SwordArtId => _weapon.SwordArtId;
        public bool Infusible => _weapon.Infusible;
        public int MaxUpgrade => _weapon.MaxUpgrade;
        public WeaponType Type => _weapon.Type;
        public AmmoType TypeAmmo => _weapon.TypeAmmo;
        //public GemViewModel DefaultGem { get; set; }
        public WeaponViewModel(Weapon weapon) : base(weapon)
        {
        }

        public override void SetupItem()
        {

        }
 
    }
}
