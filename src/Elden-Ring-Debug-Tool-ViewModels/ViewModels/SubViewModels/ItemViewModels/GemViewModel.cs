using Erd_Tools;
using Erd_Tools.Models;
using System.Collections.ObjectModel;
using static Erd_Tools.Models.Weapon;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class GemViewModel : ItemViewModel
    {
        public static ObservableCollection<GemViewModel> AllGems = new();
        protected Gem _gem => _item as Gem;
        public int SwordArtID => _gem.SwordArtID;
        public short WeaponAttr => _gem.WeaponAttr;
        public ObservableCollection<Infusion> Infusions { get; private set; }
        public ObservableCollection<WeaponType> WeaponTypes { get; private set; }
        //public static GemViewModel Default { get; private set; }
        public GemViewModel(Gem gem) : base(gem)
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
