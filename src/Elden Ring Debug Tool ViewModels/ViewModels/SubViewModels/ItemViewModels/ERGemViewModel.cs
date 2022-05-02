using Erd_Tools;
using System.Collections.ObjectModel;
using static Erd_Tools.ERWeapon;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERGemViewModel : ERItemViewModel
    {
        public static ObservableCollection<ERGemViewModel> AllGems = new ObservableCollection<ERGemViewModel>();
        protected ERGem _gem => _item as ERGem;
        public int SwordArtID => _gem.SwordArtID;
        public short WeaponAttr => _gem.WeaponAttr;
        public ObservableCollection<Infusion> Infusions { get; private set; }
        public ObservableCollection<WeaponType> WeaponTypes { get; private set; }
        //public static ERGemViewModel Default { get; private set; }
        public ERGemViewModel(ERGem gem) : base(gem)
        {
            Infusions = new ObservableCollection<Infusion>();
            WeaponTypes = new ObservableCollection<WeaponType>();
            AllGems.Add(this);
        }

        public override void SetupItem()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Infusions = new ObservableCollection<Infusion>(_gem?.Infusions);
                WeaponTypes = new ObservableCollection<WeaponType>(_gem?.WeaponTypes);
            });
        }
    }
}
