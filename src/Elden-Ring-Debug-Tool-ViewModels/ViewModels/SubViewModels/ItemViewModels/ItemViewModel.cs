using Erd_Tools;
using Erd_Tools.Models;
using System.Collections.ObjectModel;
using static Erd_Tools.Models.Item;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ItemViewModel
    {
        protected Item _item;
        public string Name => _item.Name;
        public int ID => _item.ID;
        public Category ItemCategory => _item.ItemCategory;
        public bool ShowID => _item.ShowID;

        public short MaxQuantity => _item.MaxQuantity;
        public int EventID => _item.EventID;
        public bool IsDrop => _item.IsDrop;
        public bool IsMultiplayerShare => _item.IsMultiplayerShare;
        public bool CanAquireFromOtherPlayers => IsDrop && IsMultiplayerShare;
        public static ObservableCollection<ItemViewModel> All = new();
        public int MaxUpgrade => _item is Weapon weapon ? weapon.MaxUpgrade : 0; 

        public ItemViewModel(Item item)
        {
            _item = item;
            All.Add(this);
        }

        public virtual void SetupItem()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
