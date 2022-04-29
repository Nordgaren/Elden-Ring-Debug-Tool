using Erd_Tools;
using System.Collections.ObjectModel;
using static Erd_Tools.ERItem;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERItemViewModel
    {
        protected ERItem _item;
        public string Name => _item.Name;
        public int ID => _item.ID;
        public Category ItemCategory => _item.ItemCategory;
        public bool ShowID => _item.ShowID;

        public short MaxQuantity => _item.MaxQuantity;
        public int EventID => _item.EventID;
        public bool IsDrop => _item.IsDrop;
        public bool IsMultiplayerShare => _item.IsMultiplayerShare;
        public bool CanAquireFromOtherPlayers => IsDrop && IsMultiplayerShare;
        public static ObservableCollection<ERItemViewModel> All = new ObservableCollection<ERItemViewModel>();

        public ERItemViewModel(ERItem item)
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
