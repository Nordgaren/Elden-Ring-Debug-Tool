using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erd_Tools.Models;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ContinentViewModel
    {
        public static ObservableCollection<ContinentViewModel> All { get; set; } = new();
        private Continent _continent { get; }
        public string Name { get; set; } = "";
        public ObservableCollection<HubViewModel> Hubs { get; set; } = new();
        public ContinentViewModel(Continent continent)
        {
            _continent = continent;
            Name = continent.Name;
            foreach (Hub hub in continent.Hubs)
            {
                Hubs.Add(new HubViewModel(hub, Name));
            }

            All.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class HubViewModel
    {
        public static ObservableCollection<HubViewModel> All { get; set; } = new();
        private Hub _hub { get; }
        public string Continent { get; }
        public string Name { get; }
        public ObservableCollection<GraceViewModel> Graces { get; set; } = new();

        public HubViewModel(Hub hub, string continent)
        {
            _hub = hub;
            Continent = continent;
            Name = hub.Name;
            foreach (var grace in hub.Graces)
            {
                Graces.Add(new GraceViewModel(grace));
            }
            All.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class GraceViewModel
    {
        private Grace _grace { get; }
        public static ObservableCollection<GraceViewModel> All { get; set; } = new();
        public string Continent => _grace.Continent;
        public string Hub => _grace.Hub;
        public string Name => _grace.Name;
        public List<string> Offsets => _grace.Offsets;
        public int BitStart => _grace.BitStart;

        public GraceViewModel(Grace grace)
        {
            _grace = grace;
            All.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
