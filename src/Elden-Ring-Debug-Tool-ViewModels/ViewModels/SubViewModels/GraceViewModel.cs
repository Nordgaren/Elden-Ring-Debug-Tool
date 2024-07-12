using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erd_Tools.Models;
using Erd_Tools;
using Erd_Tools.Models.Msg;
using Erd_Tools.Models.System.Dlc;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class GraceViewModel : ViewModelBase
    {
        public Grace Grace { get; }
        public static ObservableCollection<GraceViewModel> All { get; set; } = new();
        public string Continent => Grace.Continent;
        public string Hub => Grace.Hub;
        public string Name => Grace.Name;
        public int EntityID => Grace.EntityID;
        public int EventFlagID => Grace.EventFlagID;
        public DlcName Dlc => Grace.Dlc;
        private ErdHook _hook { get; }

        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (SetField(ref _enabled, value) && !_reading)
                {
                    _hook.SetEventFlag(EventFlagID, Enabled);
                }
            }
        }

        private bool _reading { get; set; }

        public GraceViewModel(Grace grace, ErdHook hook)
        {
            _hook = hook;
            Grace = grace;
            All.Add(this);
        }

        public void Update(bool enabled)
        {
            _reading = true;
            Enabled = enabled;
            _reading = false;
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
        public DlcName Dlc => _hub.Dlc;

        public HubViewModel(Hub hub, string continent, ErdHook hook)
        {
            _hub = hub;
            Continent = continent;
            Name = hub.Name;
            foreach (var grace in hub.Graces)
            {
                Graces.Add(new GraceViewModel(grace, hook));
            }

            All.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ContinentViewModel
    {
        public static ObservableCollection<ContinentViewModel> All { get; set; } = new();
        private Continent _continent { get; }
        public string Name { get; set; } = "";
        public ObservableCollection<HubViewModel> Hubs { get; set; } = new();
        public DlcName Dlc => _continent.Dlc;

        public ContinentViewModel(Continent continent, ErdHook hook)
        {
            _continent = continent;
            Name = continent.Name;
            foreach (Hub hub in continent.Hubs)
            {
                Hubs.Add(new HubViewModel(hub, Name, hook));
            }

            All.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}