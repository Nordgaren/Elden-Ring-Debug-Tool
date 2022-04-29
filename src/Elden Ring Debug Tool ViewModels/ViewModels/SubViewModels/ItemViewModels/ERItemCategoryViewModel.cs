using Erd_Tools;
using System.Collections.ObjectModel;
using static Erd_Tools.ERItem;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERItemCategoryViewModel : ViewModelBase
    {
        private ERItemCategory _erItemCategory;
        public string Name => _erItemCategory.Name;
        public ObservableCollection<ERItemViewModel> Items;
        public Category Category => _erItemCategory.Category;
        public bool ShowID => _erItemCategory.ShowID;

        public ERItemCategoryViewModel(ERItemCategory erItemCategory)
        {
            _erItemCategory = erItemCategory;

            Items = new ObservableCollection<ERItemViewModel>();
            foreach (ERItem item in _erItemCategory.Items)
            {
                Items.Add(GetViewModel(item));
            }
        }
        private ERItemViewModel GetViewModel(ERItem item)
        {
            switch (item.ItemCategory)
            {
                case Category.Weapons:
                    return new ERWeaponViewModel(item as ERWeapon);
                    break;
                case Category.Protector:
                case Category.Accessory:
                case Category.Goods:
                    return new ERItemViewModel(item);
                    break;
                case Category.Gem:
                    return new ERGemViewModel(item as ERGem);
                    break;
                default:
                    throw new Exception("Incorrect Category");
            }
        }
        public override string ToString()
        {
            return Name; 
        }
    }
}
