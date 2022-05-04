using Erd_Tools.Models;
using Erd_Tools.Models;
using System.Collections.ObjectModel;
using static Erd_Tools.Models.Item;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ItemCategoryViewModel : ViewModelBase
    {
        private ItemCategory _ItemCategory;
        public string Name => _ItemCategory.Name;
        public ObservableCollection<ItemViewModel> Items;
        public Category Category => _ItemCategory.Category;
        public bool ShowID => _ItemCategory.ShowID;

        public ItemCategoryViewModel(ItemCategory ItemCategory)
        {
            _ItemCategory = ItemCategory;

            Items = new ObservableCollection<ItemViewModel>();
            foreach (Item item in _ItemCategory.Items)
            {
                Items.Add(GetViewModel(item));
            }
        }
        private ItemViewModel GetViewModel(Item item)
        {
            switch (item.ItemCategory)
            {
                case Category.Weapons:
                    return new WeaponViewModel(item as Weapon);
                case Category.Protector:
                case Category.Accessory:
                case Category.Goods:
                    return new ItemViewModel(item);
                case Category.Gem:
                    return new GemViewModel(item as Gem);
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
